using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetLightning.Core.Utils;
using NRustLightning.Infrastructure.Entities;
using NRustLightning.Infrastructure.Models.Request;
using NRustLightning.Infrastructure.Networks;
using NRustLightning.RustLightningTypes;
using NRustLightning.Server.Interfaces;

namespace NRustLightning.Server.Services
{
    public class ChannelProvider
    {
        private Dictionary<string, Channel<FeeRateSet>> _feeRateChannels = new Dictionary<string, Channel<FeeRateSet>>();
        private Dictionary<string, Channel<PeerConnectionString>> _outboundConnectionRequestChannel = new Dictionary<string, Channel<PeerConnectionString>>();
        private Dictionary<string, Channel<SpendableOutputDescriptor>> _spendableOutputDescriptorChannel = new Dictionary<string, Channel<SpendableOutputDescriptor>>();
        public ChannelProvider(INBXplorerClientProvider clientProvider, NRustLightningNetworkProvider networkProvider)
        {
            foreach (var n in networkProvider.GetAll())
            {
                var maybeClient = clientProvider.TryGetClient(n.CryptoCode);
                if (maybeClient != null)
                {
                    _feeRateChannels.Add(n.CryptoCode, Channel.CreateBounded<FeeRateSet>(50));
                    _outboundConnectionRequestChannel.Add(n.CryptoCode, Channel.CreateBounded<PeerConnectionString>(1000));
                    _spendableOutputDescriptorChannel.Add(n.CryptoCode, Channel.CreateBounded<SpendableOutputDescriptor>(100000));
                }
            }

        }

        public Channel<FeeRateSet> GetFeeRateChannel(NRustLightningNetwork n)
        {
            if (!_feeRateChannels.TryGetValue(n.CryptoCode, out var feeRateChannel))
            {
                throw new Exception($"Channel not found for {n.CryptoCode}! this should never happen");
            }
            return feeRateChannel;
        }

        public Channel<PeerConnectionString> GetOutboundConnectionRequestQueue(string cryptoCode)
        {
            if (!_outboundConnectionRequestChannel.TryGetValue(cryptoCode, out var channel))
            {
                throw new Exception($"Channel not found for {cryptoCode}! this should never happen");
            }

            return channel;
        }

        public Channel<SpendableOutputDescriptor> GetSpendableOutputDescriptorChannel(string cryptoCode)
        {
            if (!_spendableOutputDescriptorChannel.TryGetValue(cryptoCode, out var channel))
            {
                throw new Exception($"Channel not found for {cryptoCode}! this should never happen");
            }

            return channel;
        }

        public Channel<PeerConnectionString> GetOutboundConnectionRequestQueue(NRustLightningNetwork n) =>
            GetOutboundConnectionRequestQueue(n.CryptoCode);
    }

    public class DataFlowProvider
    {
        private Dictionary<string, ISourceBlock<FeeRateSet>> _feeRateSourceBlock = new Dictionary<string, ISourceBlock<FeeRateSet>>();

        public DataFlowProvider(INBXplorerClientProvider clientProvider, NRustLightningNetworkProvider networkProvider)
        {
            foreach (var n in networkProvider.GetAll())
            {
                var maybeClient = clientProvider.TryGetClient(n.CryptoCode);
                if (maybeClient != null)
                {
                    var inputBuffer = new BufferBlock<FeeRateSet>();
                    _feeRateSourceBlock.Add(n.CryptoCode, inputBuffer);
                    throw new NotImplementedException();
                }
            }
        }

    }
}