using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin.DataEncoders;
using NBitcoin.RPC;
using Newtonsoft.Json.Linq;

namespace NBitcoin.Scripting.Utils
{
	internal static class KeyPath
	{
		 public static NBitcoin.KeyPath Empty => new NBitcoin.KeyPath(new uint[0]);
		 public static string ToStringWithEmptyKeyPathAware(this RootedKeyPath rootedKeyPath) => rootedKeyPath.KeyPath == Empty ? rootedKeyPath.MasterFingerprint.ToString() : rootedKeyPath.ToString();
	}

	internal static class HDFingerprint
	{
		public static NBitcoin.HDFingerprint FromKeyId(KeyId id)
		{
			return new NBitcoin.HDFingerprint(id.ToBytes().Take(4).ToArray());
		}
	}
	
	internal static class ODExtensions
	{
		/// <summary>
		/// When we store internal ScriptId -> Script lookup, having another
		/// WitScriptId -> WitScript KVMap will complicate implementation. And require
		/// More space because WitScriptId is bigger than ScriptId. But if we use Hash160 as ID,
		/// It will cause a problem in case of p2sh-p2wsh because we must hold two scripts
		/// (witness program and witness script) with one ScriptId. So instead we use single-RIPEMD160
		/// This is the same way with how bitcoin core handles scripts internally.
		/// </summary>
		public static ScriptId HashForLookUp(this WitScriptId witScriptId)
		{
			return new ScriptId(new uint160(NBitcoin.Crypto.Hashes.RIPEMD160(witScriptId.ToBytes(), 0, 20)));
		}
	}

	public static class RPCClientExtensions
	{
		public static ScanTxoutSetResponse StartScanTxoutSet(this RPCClient c, OutputDescriptor descriptor, uint rangeStart = 0,
			uint rangeEnd = 1000) => c.StartScanTxoutSetAsync(new[] {descriptor}.AsEnumerable(), rangeStart, rangeEnd).GetAwaiter().GetResult();

		public static ScanTxoutSetResponse StartScanTxoutSet(this RPCClient c, IEnumerable<OutputDescriptor> descriptor,
			uint rangeStart = 0, uint rangeEnd = 1000)
			=> c.StartScanTxoutSetAsync(descriptor, rangeStart, rangeEnd).GetAwaiter().GetResult();

		public static Task<ScanTxoutSetResponse> StartScanTxoutSetAsync(this RPCClient c, OutputDescriptor descriptor, uint rangeStart = 0,
			uint rangeEnd = 1000) => c.StartScanTxoutSetAsync(new[] {descriptor}.AsEnumerable(), rangeStart, rangeEnd);
		public static async Task<ScanTxoutSetResponse> StartScanTxoutSetAsync(this RPCClient c, IEnumerable<OutputDescriptor> descriptor, uint rangeStart = 0, uint rangeEnd = 1000)
		{
			if (descriptor == null)
				throw new ArgumentNullException(nameof(descriptor));

			JArray descriptorsJson = new JArray();
			foreach (var descObj in descriptor)
			{
				JObject descJson = new JObject();
				descJson.Add(new JProperty("desc", descObj.ToString()));
				if (descObj.IsRange())
				{
					var r = new JArray();
					r.Add(rangeStart);
					r.Add(rangeEnd);
					descJson.Add(new JProperty("range", r));
				}
				descriptorsJson.Add(descJson);
			}

			var result = await c.SendCommandAsync(RPCOperations.scantxoutset, "start", descriptorsJson);
			result.ThrowIfError();

			var jobj = result.Result as JObject;
			var amount = Money.Coins(jobj.Property("total_amount").Value.Value<decimal>());
			var success = jobj.Property("success").Value.Value<bool>();
			//searched_items

			var searchedItems = (int)(jobj.Property("txouts") ?? jobj.Property("searched_items")).Value.Value<long>();
			var outputs = new List<ScanTxoutOutput>();
			foreach (var unspent in (jobj.Property("unspents").Value as JArray).OfType<JObject>())
			{
				OutPoint outpoint = OutPoint.Parse($"{unspent.Property("txid").Value.Value<string>()}-{(int)unspent.Property("vout").Value.Value<long>()}");
				var a = Money.Coins(unspent.Property("amount").Value.Value<decimal>());
				int height = (int)unspent.Property("height").Value.Value<long>();
				var scriptPubKey = Script.FromBytesUnsafe(Encoders.Hex.DecodeData(unspent.Property("scriptPubKey").Value.Value<string>()));
				outputs.Add(new ScanTxoutOutput()
				{
					Coin = new Coin(outpoint, new TxOut(a, scriptPubKey)),
					Height = height
				});
			}

			return new ScanTxoutSetResponse()
			{
				Outputs = outputs.ToArray(),
				TotalAmount = amount,
				Success = success,
				SearchedItems = searchedItems
			};
		}
	}
	public class ScanTxoutSetResponse
	{
		public int SearchedItems { get; internal set; }
		public bool Success { get; internal set; }
		public ScanTxoutOutput[] Outputs { get; set; }
		public Money TotalAmount { get; set; }
	}

}