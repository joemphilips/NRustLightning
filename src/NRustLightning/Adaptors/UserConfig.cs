using System;
using System.Runtime.InteropServices;

namespace NRustLightning.Adaptors
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ChannelConfig
	{
		/// Amount (in millionths of a satoshi) the channel will charge per transferred satoshi.
		/// This may be allowed to change at runtime in a later update, however doing so must result in
		/// update messages sent to notify all nodes of our updated relay fee.
		///
		/// Default value: 0.
		public uint FeeProportionalMillionths { get; set; }

		/// Set to announce the channel publicly and notify all nodes that they can route via this
		/// channel.
		///
		/// This should only be set to true for nodes which expect to be online reliably.
		///
		/// As the node which funds a channel picks this value this will only apply for new outbound
		/// channels unless ChannelHandshakeLimits::force_announced_channel_preferences is set.
		///
		/// This cannot be changed after the initial channel handshake.
		///
		/// Default value: false.
		public byte AnnouncedChannel { get; set; }

		/// When set, we commit to an upfront shutdown_pubkey at channel open. If our counterparty
		/// supports it, they will then enforce the mutual-close output to us matches what we provided
		/// at intialization, preventing us from closing to an alternate pubkey.
		///
		/// This is set to true by default to provide a slight increase in security, though ultimately
		/// any attacker who is able to take control of a channel can just as easily send the funds via
		/// lightning payments, so we never require that our counterparties support this option.
		///
		/// This cannot be changed after a channel has been initialized.
		///
		/// Default value: true.
		public byte CommitUpfrontShutdownPubkey { get; set; }

		public static ChannelConfig GetDefault()
		{
			return new ChannelConfig
			{
				FeeProportionalMillionths = 0,
				AnnouncedChannel = 0,
				CommitUpfrontShutdownPubkey = 1,
			};
		}
	}
	[StructLayout(LayoutKind.Sequential)]
    public struct ChannelHandshakeConfig
    {
	    /// Confirmations we will wait for before considering the channel locked in.
        /// Applied only for inbound channels (see ChannelHandshakeLimits::max_minimum_depth for the
        /// equivalent limit applied to outbound channels).
        ///
        /// Default value: 6.
        public uint MinimumDepth { get; set; }

	    /// Set to the amount of time we require our counterparty to wait to claim their money.
        ///
        /// It's one of the main parameter of our security model. We (or one of our watchtowers) MUST
        /// be online to check for peer having broadcast a revoked transaction to steal our funds
        /// at least once every our_to_self_delay blocks.
        ///
        /// Meanwhile, asking for a too high delay, we bother peer to freeze funds for nothing in
        /// case of an honest unilateral channel close, which implicitly decrease the economic value of
        /// our channel.
        ///
        /// Default value: BREAKDOWN_TIMEOUT (currently 144), we enforce it as a minimum at channel
        /// opening so you can tweak config to ask for more security, not less.
        public ushort OurToSelfDelay { get; set; }

	    /// Set to the smallest value HTLC we will accept to process.
        ///
        /// This value is sent to our counterparty on channel-open and we close the channel any time
        /// our counterparty misbehaves by sending us an HTLC with a value smaller than this.
        ///
        /// Default value: 1. If the value is less than 1, it is ignored and set to 1, as is required
        /// by the protocol.
        public ulong OurHtlcMinimumMsat { get; set; }

        public static ChannelHandshakeConfig GetDefault()
        {
	        return new ChannelHandshakeConfig
	        {
		        MinimumDepth = 6,
		        OurToSelfDelay = 6 * 24,
		        OurHtlcMinimumMsat = 1,
	        };
        }
    }

	[StructLayout(LayoutKind.Sequential)]
    public struct ChannelHandshakeLimits
    {
	    /// Minimum allowed satoshis when a channel is funded, this is supplied by the sender and so
	    /// only applies to inbound channels.
	    ///
	    /// Default value: 0.
	    public ulong MinFundingSatoshis { get; set; }

	    /// The remote node sets a limit on the minimum size of HTLCs we can send to them. This allows
	    /// you to limit the maximum minimum-size they can require.
	    ///
	    /// Default value: u64::max_value.
	    public ulong MaxHtlcMinimumMsat { get; set; }

	    /// The remote node sets a limit on the maximum value of pending HTLCs to them at any given
	    /// time to limit their funds exposure to HTLCs. This allows you to set a minimum such value.
	    ///
	    /// Default value: 0.
	    public ulong MinMaxHtlcValueInFlightMsat { get; set; }

	    /// The remote node will require we keep a certain amount in direct payment to ourselves at all
	    /// time, ensuring that we are able to be punished if we broadcast an old state. This allows to
	    /// you limit the amount which we will have to keep to ourselves (and cannot use for HTLCs).
	    ///
	    /// Default value: u64::max_value.
	    public ulong MaxChannelReserveSatoshis { get; set; }

	    /// The remote node sets a limit on the maximum number of pending HTLCs to them at any given
	    /// time. This allows you to set a minimum such value.
	    ///
	    /// Default value: 0.
	    public ushort MinMaxAcceptedHtlcs { get; set; }

	    /// Outputs below a certain value will not be added to on-chain transactions. The dust value is
	    /// required to always be higher than this value so this only applies to HTLC outputs (and
	    /// potentially to-self outputs before any payments have been made).
	    /// Thus, HTLCs below this amount plus HTLC transaction fees are not enforceable on-chain.
	    /// This setting allows you to set a minimum dust limit for their commitment transactions,
	    /// reflecting the reality that tiny outputs are not considered standard transactions and will
	    /// not propagate through the Bitcoin network.
	    ///
	    /// Default value: 546, the current dust limit on the Bitcoin network.
	    public ulong MinDustLimitSatoshis { get; set; }

	    /// Maximum allowed threshold above which outputs will not be generated in their commitment
	    /// transactions.
	    /// HTLCs below this amount plus HTLC transaction fees are not enforceable on-chain.
	    ///
	    /// Default value: u64::max_value.
	    public ulong MaxDustLimitSatoshis { get; set; }

	    /// Before a channel is usable the funding transaction will need to be confirmed by at least a
	    /// certain number of blocks, specified by the node which is not the funder (as the funder can
	    /// assume they aren't going to double-spend themselves).
	    /// This config allows you to set a limit on the maximum amount of time to wait.
	    ///
	    /// Default value: 144, or roughly one day and only applies to outbound channels.
	    public uint MaxMinimumDepth { get; set; }

	    /// Set to force the incoming channel to match our announced channel preference in
	    /// ChannelConfig.
	    ///
	    /// Default value: true, to make the default that no announced channels are possible (which is
	    /// appropriate for any nodes which are not online very reliably).
	    public byte ForceAnnouncedChannelPreference { get; set; }

	    /// Set to the amount of time we're willing to wait to claim money back to us.
	    ///
	    /// Not checking this value would be a security issue, as our peer would be able to set it to
	    /// max relative lock-time (a year) and we would "lose" money as it would be locked for a long time.
	    ///
	    /// Default value: MAX_LOCAL_BREAKDOWN_TIMEOUT (1008), which we also enforce as a maximum value
	    /// so you can tweak config to reduce the loss of having useless locked funds (if your peer accepts)
	    public ushort TheirToSelfDelay { get; set; }

	    public static ChannelHandshakeLimits GetDefault()
	    {
		    return new ChannelHandshakeLimits
		    {
			    MinFundingSatoshis = 0,
			    MaxHtlcMinimumMsat = UInt64.MaxValue,
			    MinMaxHtlcValueInFlightMsat = 0,
			    MaxChannelReserveSatoshis = UInt64.MaxValue,
			    MinMaxAcceptedHtlcs = 0,
			    MinDustLimitSatoshis = 546,
			    MaxDustLimitSatoshis = UInt64.MaxValue,
			    MaxMinimumDepth = 144,
			    ForceAnnouncedChannelPreference = 1,
			    TheirToSelfDelay = 6 * 24 * 7
		    };
	    }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserConfig
    {
	    /// Channel config that we propose to our counterparty.
	    public ChannelHandshakeConfig OwnChannelConfig { get; set; }

	    /// Limits applied to our counterparty's proposed channel config settings.
	    public ChannelHandshakeLimits PeerChannelConfigLimits { get; set; }

	    /// Channel config which affects behavior during channel lifetime.
	    public ChannelConfig ChannelOptions { get; set; }

	    public static UserConfig GetDefault()
	    {
		    return new UserConfig
		    {
			    OwnChannelConfig =  ChannelHandshakeConfig.GetDefault(),
			    PeerChannelConfigLimits = ChannelHandshakeLimits.GetDefault(),
			    ChannelOptions = ChannelConfig.GetDefault()
		    };
	    }
    }
}