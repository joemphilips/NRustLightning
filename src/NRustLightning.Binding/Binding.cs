using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace NRustLightning.Binding
{
    public partial struct nativeChannelHandshakeConfigOpaque
    {
    }

    public partial struct nativeChannelHandshakeLimitsOpaque
    {
    }

    public partial struct nativeChannelConfigOpaque
    {
    }

    public partial struct nativeUserConfigOpaque
    {
    }

    public partial struct nativeChainWatchedUtilOpaque
    {
    }

    public partial struct nativeBlockNotifierOpaque
    {
    }

    public partial struct nativeChainWatchInterfaceUtilOpaque
    {
    }

    public partial struct nativeOutPointOpaque
    {
    }

    public partial struct LDKChannelKeys
    {
    }

    public unsafe partial struct LDKChannelKeys
    {
        [NativeTypeName("void *")]
        public void* this_arg;

        [NativeTypeName("LDKPublicKey (*)(const void *, uint64_t)")]
        public IntPtr get_per_commitment_point;

        [NativeTypeName("LDKThirtyTwoBytes (*)(const void *, uint64_t)")]
        public IntPtr release_commitment_secret;

        public LDKChannelPublicKeys pubkeys;

        [NativeTypeName("void (*)(const LDKChannelKeys *)")]
        public IntPtr set_pubkeys;

        [NativeTypeName("LDKC2Tuple_u64u64Z (*)(const void *)")]
        public IntPtr key_derivation_params;

        [NativeTypeName("LDKCResult_C2Tuple_SignatureCVec_SignatureZZNoneZ (*)(const void *, uint32_t, LDKTransaction, const LDKPreCalculatedTxCreationKeys *, LDKCVec_HTLCOutputInCommitmentZ)")]
        public IntPtr sign_remote_commitment;

        [NativeTypeName("LDKCResult_SignatureNoneZ (*)(const void *, const LDKLocalCommitmentTransaction *)")]
        public IntPtr sign_local_commitment;

        [NativeTypeName("LDKCResult_CVec_SignatureZNoneZ (*)(const void *, const LDKLocalCommitmentTransaction *)")]
        public IntPtr sign_local_commitment_htlc_transactions;

        [NativeTypeName("LDKCResult_SignatureNoneZ (*)(const void *, LDKTransaction, uintptr_t, uint64_t, const uint8_t (*)[32], const LDKHTLCOutputInCommitment *)")]
        public IntPtr sign_justice_transaction;

        [NativeTypeName("LDKCResult_SignatureNoneZ (*)(const void *, LDKTransaction, uintptr_t, uint64_t, LDKPublicKey, const LDKHTLCOutputInCommitment *)")]
        public IntPtr sign_remote_htlc_transaction;

        [NativeTypeName("LDKCResult_SignatureNoneZ (*)(const void *, LDKTransaction)")]
        public IntPtr sign_closing_transaction;

        [NativeTypeName("LDKCResult_SignatureNoneZ (*)(const void *, const LDKUnsignedChannelAnnouncement *)")]
        public IntPtr sign_channel_announcement;

        [NativeTypeName("void (*)(void *, const LDKChannelPublicKeys *, uint16_t, uint16_t)")]
        public IntPtr on_accept;

        [NativeTypeName("void *(*)(const void *)")]
        public IntPtr clone;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr free;
    }

    public partial struct nativeInMemoryChannelKeysOpaque
    {
    }

    public partial struct nativeKeysManagerOpaque
    {
    }

    public partial struct nativeChannelManagerOpaque
    {
    }

    public partial struct nativeChannelDetailsOpaque
    {
    }

    public partial struct nativePaymentSendFailureOpaque
    {
    }

    public partial struct nativeChannelManagerReadArgsOpaque
    {
    }

    public partial struct nativeChannelMonitorUpdateOpaque
    {
    }

    public partial struct nativeMonitorUpdateErrorOpaque
    {
    }

    public partial struct nativeMonitorEventOpaque
    {
    }

    public partial struct nativeHTLCUpdateOpaque
    {
    }

    public partial struct nativeChannelMonitorOpaque
    {
    }

    public partial struct nativeDecodeErrorOpaque
    {
    }

    public partial struct nativeInitOpaque
    {
    }

    public partial struct nativeErrorMessageOpaque
    {
    }

    public partial struct nativePingOpaque
    {
    }

    public partial struct nativePongOpaque
    {
    }

    public partial struct nativeOpenChannelOpaque
    {
    }

    public partial struct nativeAcceptChannelOpaque
    {
    }

    public partial struct nativeFundingCreatedOpaque
    {
    }

    public partial struct nativeFundingSignedOpaque
    {
    }

    public partial struct nativeFundingLockedOpaque
    {
    }

    public partial struct nativeShutdownOpaque
    {
    }

    public partial struct nativeClosingSignedOpaque
    {
    }

    public partial struct nativeUpdateAddHTLCOpaque
    {
    }

    public partial struct nativeUpdateFulfillHTLCOpaque
    {
    }

    public partial struct nativeUpdateFailHTLCOpaque
    {
    }

    public partial struct nativeUpdateFailMalformedHTLCOpaque
    {
    }

    public partial struct nativeCommitmentSignedOpaque
    {
    }

    public partial struct nativeRevokeAndACKOpaque
    {
    }

    public partial struct nativeUpdateFeeOpaque
    {
    }

    public partial struct nativeDataLossProtectOpaque
    {
    }

    public partial struct nativeChannelReestablishOpaque
    {
    }

    public partial struct nativeAnnouncementSignaturesOpaque
    {
    }

    public partial struct nativeUnsignedNodeAnnouncementOpaque
    {
    }

    public partial struct nativeNodeAnnouncementOpaque
    {
    }

    public partial struct nativeUnsignedChannelAnnouncementOpaque
    {
    }

    public partial struct nativeChannelAnnouncementOpaque
    {
    }

    public partial struct nativeUnsignedChannelUpdateOpaque
    {
    }

    public partial struct nativeChannelUpdateOpaque
    {
    }

    public partial struct nativeLightningErrorOpaque
    {
    }

    public partial struct nativeCommitmentUpdateOpaque
    {
    }

    public partial struct nativeMessageHandlerOpaque
    {
    }

    public partial struct nativePeerHandleErrorOpaque
    {
    }

    public partial struct nativePeerManagerOpaque
    {
    }

    public partial struct nativeTxCreationKeysOpaque
    {
    }

    public partial struct nativePreCalculatedTxCreationKeysOpaque
    {
    }

    public partial struct nativeChannelPublicKeysOpaque
    {
    }

    public partial struct nativeHTLCOutputInCommitmentOpaque
    {
    }

    public partial struct nativeLocalCommitmentTransactionOpaque
    {
    }

    public partial struct nativeInitFeaturesOpaque
    {
    }

    public partial struct nativeNodeFeaturesOpaque
    {
    }

    public partial struct nativeChannelFeaturesOpaque
    {
    }

    public partial struct nativeRouteHopOpaque
    {
    }

    public partial struct nativeRouteOpaque
    {
    }

    public partial struct nativeRouteHintOpaque
    {
    }

    public partial struct nativeNetworkGraphOpaque
    {
    }

    public partial struct nativeLockedNetworkGraphOpaque
    {
    }

    public partial struct nativeNetGraphMsgHandlerOpaque
    {
    }

    public partial struct nativeDirectionalChannelInfoOpaque
    {
    }

    public partial struct nativeChannelInfoOpaque
    {
    }

    public partial struct nativeRoutingFeesOpaque
    {
    }

    public partial struct nativeNodeAnnouncementInfoOpaque
    {
    }

    public partial struct nativeNodeInfoOpaque
    {
    }

    [NativeTypeName("unsigned int")]
    public enum LDKChainError : uint
    {
        LDKChainError_NotSupported,
        LDKChainError_NotWatched,
        LDKChainError_UnknownTx,
        LDKChainError_Sentinel,
    }

    [NativeTypeName("unsigned int")]
    public enum LDKChannelMonitorUpdateErr : uint
    {
        LDKChannelMonitorUpdateErr_TemporaryFailure,
        LDKChannelMonitorUpdateErr_PermanentFailure,
        LDKChannelMonitorUpdateErr_Sentinel,
    }

    [NativeTypeName("unsigned int")]
    public enum LDKConfirmationTarget : uint
    {
        LDKConfirmationTarget_Background,
        LDKConfirmationTarget_Normal,
        LDKConfirmationTarget_HighPriority,
        LDKConfirmationTarget_Sentinel,
    }

    [NativeTypeName("unsigned int")]
    public enum LDKLevel : uint
    {
        LDKLevel_Off,
        LDKLevel_Error,
        LDKLevel_Warn,
        LDKLevel_Info,
        LDKLevel_Debug,
        LDKLevel_Trace,
        LDKLevel_Sentinel,
    }

    [NativeTypeName("unsigned int")]
    public enum LDKNetwork : uint
    {
        LDKNetwork_Bitcoin,
        LDKNetwork_Testnet,
        LDKNetwork_Regtest,
        LDKNetwork_Sentinel,
    }

    [NativeTypeName("unsigned int")]
    public enum LDKSecp256k1Error : uint
    {
        LDKSecp256k1Error_IncorrectSignature,
        LDKSecp256k1Error_InvalidMessage,
        LDKSecp256k1Error_InvalidPublicKey,
        LDKSecp256k1Error_InvalidSignature,
        LDKSecp256k1Error_InvalidSecretKey,
        LDKSecp256k1Error_InvalidRecoveryId,
        LDKSecp256k1Error_InvalidTweak,
        LDKSecp256k1Error_NotEnoughMemory,
        LDKSecp256k1Error_CallbackPanicked,
        LDKSecp256k1Error_Sentinel,
    }

    public unsafe partial struct LDKCVecTempl_u8
    {
        [NativeTypeName("uint8_t *")]
        public byte* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public partial struct LDKTxOut
    {
        [NativeTypeName("LDKCVec_u8Z")]
        public LDKCVecTempl_u8 script_pubkey;

        [NativeTypeName("uint64_t")]
        public ulong value;
    }

    public unsafe partial struct LDKThirtyTwoBytes
    {
        [NativeTypeName("uint8_t [32]")]
        public fixed byte data[32];
    }

    public unsafe partial struct LDKC2TupleTempl_ThirtyTwoBytes__u32
    {
        [NativeTypeName("LDKThirtyTwoBytes *")]
        public LDKThirtyTwoBytes* a;

        [NativeTypeName("uint32_t *")]
        public uint* b;
    }

    public unsafe partial struct LDKC2TupleTempl_CVec_u8Z__u64
    {
        [NativeTypeName("LDKCVec_u8Z *")]
        public LDKCVecTempl_u8* a;

        [NativeTypeName("uint64_t *")]
        public ulong* b;
    }

    public unsafe partial struct LDKC2TupleTempl_u64__u64
    {
        [NativeTypeName("uint64_t *")]
        public ulong* a;

        [NativeTypeName("uint64_t *")]
        public ulong* b;
    }

    public unsafe partial struct LDKSignature
    {
        [NativeTypeName("uint8_t [64]")]
        public fixed byte compact_form[64];
    }

    public unsafe partial struct LDKCVecTempl_Signature
    {
        [NativeTypeName("LDKSignature *")]
        public LDKSignature* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKC2TupleTempl_Signature__CVecTempl_Signature
    {
        [NativeTypeName("LDKSignature *")]
        public LDKSignature* a;

        [NativeTypeName("LDKCVecTempl_Signature *")]
        public LDKCVecTempl_Signature* b;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_C2TupleTempl_Signature__CVecTempl_Signature________u8
    {
        [FieldOffset(0)]
        [NativeTypeName("LDKC2TupleTempl_Signature__CVecTempl_Signature *")]
        public LDKC2TupleTempl_Signature__CVecTempl_Signature* result;

        [FieldOffset(0)]
        [NativeTypeName("uint8_t *")]
        public byte* err;
    }

    public partial struct LDKCResultTempl_C2TupleTempl_Signature__CVecTempl_Signature________u8
    {
        public LDKCResultPtr_C2TupleTempl_Signature__CVecTempl_Signature________u8 contents;

        public bool result_ok;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_Signature__u8
    {
        [FieldOffset(0)]
        [NativeTypeName("LDKSignature *")]
        public LDKSignature* result;

        [FieldOffset(0)]
        [NativeTypeName("uint8_t *")]
        public byte* err;
    }

    public partial struct LDKCResultTempl_Signature__u8
    {
        public LDKCResultPtr_Signature__u8 contents;

        public bool result_ok;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_CVecTempl_Signature_____u8
    {
        [FieldOffset(0)]
        [NativeTypeName("LDKCVecTempl_Signature *")]
        public LDKCVecTempl_Signature* result;

        [FieldOffset(0)]
        [NativeTypeName("uint8_t *")]
        public byte* err;
    }

    public partial struct LDKCResultTempl_CVecTempl_Signature_____u8
    {
        public LDKCResultPtr_CVecTempl_Signature_____u8 contents;

        public bool result_ok;
    }

    public unsafe partial struct LDKStr
    {
        [NativeTypeName("const uint8_t *")]
        public byte* chars;

        [NativeTypeName("uintptr_t")]
        public UIntPtr len;
    }

    [NativeTypeName("unsigned int")]
    public enum LDKAPIError_Tag : uint
    {
        LDKAPIError_APIMisuseError,
        LDKAPIError_FeeRateTooHigh,
        LDKAPIError_RouteError,
        LDKAPIError_ChannelUnavailable,
        LDKAPIError_MonitorUpdateFailed,
        LDKAPIError_Sentinel,
    }

    public partial struct LDKAPIError_LDKAPIMisuseError_Body
    {
        [NativeTypeName("LDKCVec_u8Z")]
        public LDKCVecTempl_u8 err;
    }

    public partial struct LDKAPIError_LDKFeeRateTooHigh_Body
    {
        [NativeTypeName("LDKCVec_u8Z")]
        public LDKCVecTempl_u8 err;

        [NativeTypeName("uint32_t")]
        public uint feerate;
    }

    public partial struct LDKAPIError_LDKRouteError_Body
    {
        public LDKStr err;
    }

    public partial struct LDKAPIError_LDKChannelUnavailable_Body
    {
        [NativeTypeName("LDKCVec_u8Z")]
        public LDKCVecTempl_u8 err;
    }

    public partial struct LDKAPIError
    {
        public LDKAPIError_Tag tag;

        [NativeTypeName("LDKAPIError::(anonymous union at ./rust-lightning/lightning-c-bindings/include/lightning.h:340:4)")]
        public _Anonymous_e__Union Anonymous;

        public ref LDKAPIError_LDKAPIMisuseError_Body api_misuse_error
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.api_misuse_error, 1));
            }
        }

        public ref LDKAPIError_LDKFeeRateTooHigh_Body fee_rate_too_high
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.fee_rate_too_high, 1));
            }
        }

        public ref LDKAPIError_LDKRouteError_Body route_error
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.route_error, 1));
            }
        }

        public ref LDKAPIError_LDKChannelUnavailable_Body channel_unavailable
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.channel_unavailable, 1));
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public LDKAPIError_LDKAPIMisuseError_Body api_misuse_error;

            [FieldOffset(0)]
            public LDKAPIError_LDKFeeRateTooHigh_Body fee_rate_too_high;

            [FieldOffset(0)]
            public LDKAPIError_LDKRouteError_Body route_error;

            [FieldOffset(0)]
            public LDKAPIError_LDKChannelUnavailable_Body channel_unavailable;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_u8__APIError
    {
        [FieldOffset(0)]
        [NativeTypeName("uint8_t *")]
        public byte* result;

        [FieldOffset(0)]
        [NativeTypeName("LDKAPIError *")]
        public LDKAPIError* err;
    }

    public partial struct LDKCResultTempl_u8__APIError
    {
        public LDKCResultPtr_u8__APIError contents;

        public bool result_ok;
    }

    public unsafe partial struct LDKPaymentSendFailure
    {
        [NativeTypeName("LDKnativePaymentSendFailure *")]
        public nativePaymentSendFailureOpaque* inner;

        public bool is_owned;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_u8__PaymentSendFailure
    {
        [FieldOffset(0)]
        [NativeTypeName("uint8_t *")]
        public byte* result;

        [FieldOffset(0)]
        [NativeTypeName("LDKPaymentSendFailure *")]
        public LDKPaymentSendFailure* err;
    }

    public partial struct LDKCResultTempl_u8__PaymentSendFailure
    {
        public LDKCResultPtr_u8__PaymentSendFailure contents;

        public bool result_ok;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_u8__ChannelMonitorUpdateErr
    {
        [FieldOffset(0)]
        [NativeTypeName("uint8_t *")]
        public byte* result;

        [FieldOffset(0)]
        [NativeTypeName("LDKChannelMonitorUpdateErr *")]
        public LDKChannelMonitorUpdateErr* err;
    }

    public partial struct LDKCResultTempl_u8__ChannelMonitorUpdateErr
    {
        public LDKCResultPtr_u8__ChannelMonitorUpdateErr contents;

        public bool result_ok;
    }

    public unsafe partial struct LDKMonitorUpdateError
    {
        [NativeTypeName("LDKnativeMonitorUpdateError *")]
        public nativeMonitorUpdateErrorOpaque* inner;

        public bool is_owned;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_u8__MonitorUpdateError
    {
        [FieldOffset(0)]
        [NativeTypeName("uint8_t *")]
        public byte* result;

        [FieldOffset(0)]
        [NativeTypeName("LDKMonitorUpdateError *")]
        public LDKMonitorUpdateError* err;
    }

    public partial struct LDKCResultTempl_u8__MonitorUpdateError
    {
        public LDKCResultPtr_u8__MonitorUpdateError contents;

        public bool result_ok;
    }

    public unsafe partial struct LDKOutPoint
    {
        [NativeTypeName("LDKnativeOutPoint *")]
        public nativeOutPointOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKC2TupleTempl_OutPoint__CVec_u8Z
    {
        [NativeTypeName("LDKOutPoint *")]
        public LDKOutPoint* a;

        [NativeTypeName("LDKCVec_u8Z *")]
        public LDKCVecTempl_u8* b;
    }

    public unsafe partial struct LDKChannelAnnouncement
    {
        [NativeTypeName("LDKnativeChannelAnnouncement *")]
        public nativeChannelAnnouncementOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKChannelUpdate
    {
        [NativeTypeName("LDKnativeChannelUpdate *")]
        public nativeChannelUpdateOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKC3TupleTempl_ChannelAnnouncement__ChannelUpdate__ChannelUpdate
    {
        [NativeTypeName("LDKChannelAnnouncement *")]
        public LDKChannelAnnouncement* a;

        [NativeTypeName("LDKChannelUpdate *")]
        public LDKChannelUpdate* b;

        [NativeTypeName("LDKChannelUpdate *")]
        public LDKChannelUpdate* c;
    }

    public unsafe partial struct LDKPeerHandleError
    {
        [NativeTypeName("LDKnativePeerHandleError *")]
        public nativePeerHandleErrorOpaque* inner;

        public bool is_owned;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_u8__PeerHandleError
    {
        [FieldOffset(0)]
        [NativeTypeName("uint8_t *")]
        public byte* result;

        [FieldOffset(0)]
        [NativeTypeName("LDKPeerHandleError *")]
        public LDKPeerHandleError* err;
    }

    public partial struct LDKCResultTempl_u8__PeerHandleError
    {
        public LDKCResultPtr_u8__PeerHandleError contents;

        public bool result_ok;
    }

    public unsafe partial struct LDKHTLCOutputInCommitment
    {
        [NativeTypeName("LDKnativeHTLCOutputInCommitment *")]
        public nativeHTLCOutputInCommitmentOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKC2TupleTempl_HTLCOutputInCommitment__Signature
    {
        [NativeTypeName("LDKHTLCOutputInCommitment *")]
        public LDKHTLCOutputInCommitment* a;

        [NativeTypeName("LDKSignature *")]
        public LDKSignature* b;
    }

    public unsafe partial struct LDKPublicKey
    {
        [NativeTypeName("uint8_t [33]")]
        public fixed byte compressed_form[33];
    }

    [NativeTypeName("unsigned int")]
    public enum LDKSpendableOutputDescriptor_Tag : uint
    {
        LDKSpendableOutputDescriptor_StaticOutput,
        LDKSpendableOutputDescriptor_DynamicOutputP2WSH,
        LDKSpendableOutputDescriptor_StaticOutputRemotePayment,
        LDKSpendableOutputDescriptor_Sentinel,
    }

    public partial struct LDKSpendableOutputDescriptor_LDKStaticOutput_Body
    {
        public LDKOutPoint outpoint;

        public LDKTxOut output;
    }

    public partial struct LDKSpendableOutputDescriptor_LDKDynamicOutputP2WSH_Body
    {
        public LDKOutPoint outpoint;

        public LDKPublicKey per_commitment_point;

        [NativeTypeName("uint16_t")]
        public ushort to_self_delay;

        public LDKTxOut output;

        [NativeTypeName("LDKC2Tuple_u64u64Z")]
        public LDKC2TupleTempl_u64__u64 key_derivation_params;

        public LDKPublicKey remote_revocation_pubkey;
    }

    public partial struct LDKSpendableOutputDescriptor_LDKStaticOutputRemotePayment_Body
    {
        public LDKOutPoint outpoint;

        public LDKTxOut output;

        [NativeTypeName("LDKC2Tuple_u64u64Z")]
        public LDKC2TupleTempl_u64__u64 key_derivation_params;
    }

    public partial struct LDKSpendableOutputDescriptor
    {
        public LDKSpendableOutputDescriptor_Tag tag;

        [NativeTypeName("LDKSpendableOutputDescriptor::(anonymous union at ./rust-lightning/lightning-c-bindings/include/lightning.h:630:4)")]
        public _Anonymous_e__Union Anonymous;

        public ref LDKSpendableOutputDescriptor_LDKStaticOutput_Body static_output
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.static_output, 1));
            }
        }

        public ref LDKSpendableOutputDescriptor_LDKDynamicOutputP2WSH_Body dynamic_output_p2wsh
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.dynamic_output_p2wsh, 1));
            }
        }

        public ref LDKSpendableOutputDescriptor_LDKStaticOutputRemotePayment_Body static_output_remote_payment
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.static_output_remote_payment, 1));
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public LDKSpendableOutputDescriptor_LDKStaticOutput_Body static_output;

            [FieldOffset(0)]
            public LDKSpendableOutputDescriptor_LDKDynamicOutputP2WSH_Body dynamic_output_p2wsh;

            [FieldOffset(0)]
            public LDKSpendableOutputDescriptor_LDKStaticOutputRemotePayment_Body static_output_remote_payment;
        }
    }

    public unsafe partial struct LDKCVecTempl_SpendableOutputDescriptor
    {
        [NativeTypeName("LDKSpendableOutputDescriptor *")]
        public LDKSpendableOutputDescriptor* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    [NativeTypeName("unsigned int")]
    public enum LDKEvent_Tag : uint
    {
        LDKEvent_FundingGenerationReady,
        LDKEvent_FundingBroadcastSafe,
        LDKEvent_PaymentReceived,
        LDKEvent_PaymentSent,
        LDKEvent_PaymentFailed,
        LDKEvent_PendingHTLCsForwardable,
        LDKEvent_SpendableOutputs,
        LDKEvent_Sentinel,
    }

    public partial struct LDKEvent_LDKFundingGenerationReady_Body
    {
        public LDKThirtyTwoBytes temporary_channel_id;

        [NativeTypeName("uint64_t")]
        public ulong channel_value_satoshis;

        [NativeTypeName("LDKCVec_u8Z")]
        public LDKCVecTempl_u8 output_script;

        [NativeTypeName("uint64_t")]
        public ulong user_channel_id;
    }

    public partial struct LDKEvent_LDKFundingBroadcastSafe_Body
    {
        public LDKOutPoint funding_txo;

        [NativeTypeName("uint64_t")]
        public ulong user_channel_id;
    }

    public partial struct LDKEvent_LDKPaymentReceived_Body
    {
        public LDKThirtyTwoBytes payment_hash;

        public LDKThirtyTwoBytes payment_secret;

        [NativeTypeName("uint64_t")]
        public ulong amt;
    }

    public partial struct LDKEvent_LDKPaymentSent_Body
    {
        public LDKThirtyTwoBytes payment_preimage;
    }

    public partial struct LDKEvent_LDKPaymentFailed_Body
    {
        public LDKThirtyTwoBytes payment_hash;

        public bool rejected_by_dest;
    }

    public partial struct LDKEvent_LDKPendingHTLCsForwardable_Body
    {
        [NativeTypeName("uint64_t")]
        public ulong time_forwardable;
    }

    public partial struct LDKEvent_LDKSpendableOutputs_Body
    {
        [NativeTypeName("LDKCVec_SpendableOutputDescriptorZ")]
        public LDKCVecTempl_SpendableOutputDescriptor outputs;
    }

    public partial struct LDKEvent
    {
        public LDKEvent_Tag tag;

        [NativeTypeName("LDKEvent::(anonymous union at ./rust-lightning/lightning-c-bindings/include/lightning.h:748:4)")]
        public _Anonymous_e__Union Anonymous;

        public ref LDKEvent_LDKFundingGenerationReady_Body funding_generation_ready
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.funding_generation_ready, 1));
            }
        }

        public ref LDKEvent_LDKFundingBroadcastSafe_Body funding_broadcast_safe
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.funding_broadcast_safe, 1));
            }
        }

        public ref LDKEvent_LDKPaymentReceived_Body payment_received
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.payment_received, 1));
            }
        }

        public ref LDKEvent_LDKPaymentSent_Body payment_sent
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.payment_sent, 1));
            }
        }

        public ref LDKEvent_LDKPaymentFailed_Body payment_failed
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.payment_failed, 1));
            }
        }

        public ref LDKEvent_LDKPendingHTLCsForwardable_Body pending_htl_cs_forwardable
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.pending_htl_cs_forwardable, 1));
            }
        }

        public ref LDKEvent_LDKSpendableOutputs_Body spendable_outputs
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.spendable_outputs, 1));
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public LDKEvent_LDKFundingGenerationReady_Body funding_generation_ready;

            [FieldOffset(0)]
            public LDKEvent_LDKFundingBroadcastSafe_Body funding_broadcast_safe;

            [FieldOffset(0)]
            public LDKEvent_LDKPaymentReceived_Body payment_received;

            [FieldOffset(0)]
            public LDKEvent_LDKPaymentSent_Body payment_sent;

            [FieldOffset(0)]
            public LDKEvent_LDKPaymentFailed_Body payment_failed;

            [FieldOffset(0)]
            public LDKEvent_LDKPendingHTLCsForwardable_Body pending_htl_cs_forwardable;

            [FieldOffset(0)]
            public LDKEvent_LDKSpendableOutputs_Body spendable_outputs;
        }
    }

    public unsafe partial struct LDKAcceptChannel
    {
        [NativeTypeName("LDKnativeAcceptChannel *")]
        public nativeAcceptChannelOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKOpenChannel
    {
        [NativeTypeName("LDKnativeOpenChannel *")]
        public nativeOpenChannelOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKFundingCreated
    {
        [NativeTypeName("LDKnativeFundingCreated *")]
        public nativeFundingCreatedOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKFundingSigned
    {
        [NativeTypeName("LDKnativeFundingSigned *")]
        public nativeFundingSignedOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKFundingLocked
    {
        [NativeTypeName("LDKnativeFundingLocked *")]
        public nativeFundingLockedOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKAnnouncementSignatures
    {
        [NativeTypeName("LDKnativeAnnouncementSignatures *")]
        public nativeAnnouncementSignaturesOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKCommitmentUpdate
    {
        [NativeTypeName("LDKnativeCommitmentUpdate *")]
        public nativeCommitmentUpdateOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKRevokeAndACK
    {
        [NativeTypeName("LDKnativeRevokeAndACK *")]
        public nativeRevokeAndACKOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKClosingSigned
    {
        [NativeTypeName("LDKnativeClosingSigned *")]
        public nativeClosingSignedOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKShutdown
    {
        [NativeTypeName("LDKnativeShutdown *")]
        public nativeShutdownOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKChannelReestablish
    {
        [NativeTypeName("LDKnativeChannelReestablish *")]
        public nativeChannelReestablishOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKNodeAnnouncement
    {
        [NativeTypeName("LDKnativeNodeAnnouncement *")]
        public nativeNodeAnnouncementOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKErrorMessage
    {
        [NativeTypeName("LDKnativeErrorMessage *")]
        public nativeErrorMessageOpaque* inner;

        public bool is_owned;
    }

    [NativeTypeName("unsigned int")]
    public enum LDKErrorAction_Tag : uint
    {
        LDKErrorAction_DisconnectPeer,
        LDKErrorAction_IgnoreError,
        LDKErrorAction_SendErrorMessage,
        LDKErrorAction_Sentinel,
    }

    public partial struct LDKErrorAction_LDKDisconnectPeer_Body
    {
        public LDKErrorMessage msg;
    }

    public partial struct LDKErrorAction_LDKSendErrorMessage_Body
    {
        public LDKErrorMessage msg;
    }

    public partial struct LDKErrorAction
    {
        public LDKErrorAction_Tag tag;

        [NativeTypeName("LDKErrorAction::(anonymous union at ./rust-lightning/lightning-c-bindings/include/lightning.h:974:4)")]
        public _Anonymous_e__Union Anonymous;

        public ref LDKErrorAction_LDKDisconnectPeer_Body disconnect_peer
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.disconnect_peer, 1));
            }
        }

        public ref LDKErrorAction_LDKSendErrorMessage_Body send_error_message
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.send_error_message, 1));
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public LDKErrorAction_LDKDisconnectPeer_Body disconnect_peer;

            [FieldOffset(0)]
            public LDKErrorAction_LDKSendErrorMessage_Body send_error_message;
        }
    }

    [NativeTypeName("unsigned int")]
    public enum LDKHTLCFailChannelUpdate_Tag : uint
    {
        LDKHTLCFailChannelUpdate_ChannelUpdateMessage,
        LDKHTLCFailChannelUpdate_ChannelClosed,
        LDKHTLCFailChannelUpdate_NodeFailure,
        LDKHTLCFailChannelUpdate_Sentinel,
    }

    public partial struct LDKHTLCFailChannelUpdate_LDKChannelUpdateMessage_Body
    {
        public LDKChannelUpdate msg;
    }

    public partial struct LDKHTLCFailChannelUpdate_LDKChannelClosed_Body
    {
        [NativeTypeName("uint64_t")]
        public ulong short_channel_id;

        public bool is_permanent;
    }

    public partial struct LDKHTLCFailChannelUpdate_LDKNodeFailure_Body
    {
        public LDKPublicKey node_id;

        public bool is_permanent;
    }

    public partial struct LDKHTLCFailChannelUpdate
    {
        public LDKHTLCFailChannelUpdate_Tag tag;

        [NativeTypeName("LDKHTLCFailChannelUpdate::(anonymous union at ./rust-lightning/lightning-c-bindings/include/lightning.h:1020:4)")]
        public _Anonymous_e__Union Anonymous;

        public ref LDKHTLCFailChannelUpdate_LDKChannelUpdateMessage_Body channel_update_message
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.channel_update_message, 1));
            }
        }

        public ref LDKHTLCFailChannelUpdate_LDKChannelClosed_Body channel_closed
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.channel_closed, 1));
            }
        }

        public ref LDKHTLCFailChannelUpdate_LDKNodeFailure_Body node_failure
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.node_failure, 1));
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public LDKHTLCFailChannelUpdate_LDKChannelUpdateMessage_Body channel_update_message;

            [FieldOffset(0)]
            public LDKHTLCFailChannelUpdate_LDKChannelClosed_Body channel_closed;

            [FieldOffset(0)]
            public LDKHTLCFailChannelUpdate_LDKNodeFailure_Body node_failure;
        }
    }

    [NativeTypeName("unsigned int")]
    public enum LDKMessageSendEvent_Tag : uint
    {
        LDKMessageSendEvent_SendAcceptChannel,
        LDKMessageSendEvent_SendOpenChannel,
        LDKMessageSendEvent_SendFundingCreated,
        LDKMessageSendEvent_SendFundingSigned,
        LDKMessageSendEvent_SendFundingLocked,
        LDKMessageSendEvent_SendAnnouncementSignatures,
        LDKMessageSendEvent_UpdateHTLCs,
        LDKMessageSendEvent_SendRevokeAndACK,
        LDKMessageSendEvent_SendClosingSigned,
        LDKMessageSendEvent_SendShutdown,
        LDKMessageSendEvent_SendChannelReestablish,
        LDKMessageSendEvent_BroadcastChannelAnnouncement,
        LDKMessageSendEvent_BroadcastNodeAnnouncement,
        LDKMessageSendEvent_BroadcastChannelUpdate,
        LDKMessageSendEvent_HandleError,
        LDKMessageSendEvent_PaymentFailureNetworkUpdate,
        LDKMessageSendEvent_Sentinel,
    }

    public partial struct LDKMessageSendEvent_LDKSendAcceptChannel_Body
    {
        public LDKPublicKey node_id;

        public LDKAcceptChannel msg;
    }

    public partial struct LDKMessageSendEvent_LDKSendOpenChannel_Body
    {
        public LDKPublicKey node_id;

        public LDKOpenChannel msg;
    }

    public partial struct LDKMessageSendEvent_LDKSendFundingCreated_Body
    {
        public LDKPublicKey node_id;

        public LDKFundingCreated msg;
    }

    public partial struct LDKMessageSendEvent_LDKSendFundingSigned_Body
    {
        public LDKPublicKey node_id;

        public LDKFundingSigned msg;
    }

    public partial struct LDKMessageSendEvent_LDKSendFundingLocked_Body
    {
        public LDKPublicKey node_id;

        public LDKFundingLocked msg;
    }

    public partial struct LDKMessageSendEvent_LDKSendAnnouncementSignatures_Body
    {
        public LDKPublicKey node_id;

        public LDKAnnouncementSignatures msg;
    }

    public partial struct LDKMessageSendEvent_LDKUpdateHTLCs_Body
    {
        public LDKPublicKey node_id;

        public LDKCommitmentUpdate updates;
    }

    public partial struct LDKMessageSendEvent_LDKSendRevokeAndACK_Body
    {
        public LDKPublicKey node_id;

        public LDKRevokeAndACK msg;
    }

    public partial struct LDKMessageSendEvent_LDKSendClosingSigned_Body
    {
        public LDKPublicKey node_id;

        public LDKClosingSigned msg;
    }

    public partial struct LDKMessageSendEvent_LDKSendShutdown_Body
    {
        public LDKPublicKey node_id;

        public LDKShutdown msg;
    }

    public partial struct LDKMessageSendEvent_LDKSendChannelReestablish_Body
    {
        public LDKPublicKey node_id;

        public LDKChannelReestablish msg;
    }

    public partial struct LDKMessageSendEvent_LDKBroadcastChannelAnnouncement_Body
    {
        public LDKChannelAnnouncement msg;

        public LDKChannelUpdate update_msg;
    }

    public partial struct LDKMessageSendEvent_LDKBroadcastNodeAnnouncement_Body
    {
        public LDKNodeAnnouncement msg;
    }

    public partial struct LDKMessageSendEvent_LDKBroadcastChannelUpdate_Body
    {
        public LDKChannelUpdate msg;
    }

    public partial struct LDKMessageSendEvent_LDKHandleError_Body
    {
        public LDKPublicKey node_id;

        public LDKErrorAction action;
    }

    public partial struct LDKMessageSendEvent_LDKPaymentFailureNetworkUpdate_Body
    {
        public LDKHTLCFailChannelUpdate update;
    }

    public partial struct LDKMessageSendEvent
    {
        public LDKMessageSendEvent_Tag tag;

        [NativeTypeName("LDKMessageSendEvent::(anonymous union at ./rust-lightning/lightning-c-bindings/include/lightning.h:1193:4)")]
        public _Anonymous_e__Union Anonymous;

        public ref LDKMessageSendEvent_LDKSendAcceptChannel_Body send_accept_channel
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.send_accept_channel, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKSendOpenChannel_Body send_open_channel
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.send_open_channel, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKSendFundingCreated_Body send_funding_created
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.send_funding_created, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKSendFundingSigned_Body send_funding_signed
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.send_funding_signed, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKSendFundingLocked_Body send_funding_locked
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.send_funding_locked, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKSendAnnouncementSignatures_Body send_announcement_signatures
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.send_announcement_signatures, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKUpdateHTLCs_Body update_htl_cs
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.update_htl_cs, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKSendRevokeAndACK_Body send_revoke_and_ack
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.send_revoke_and_ack, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKSendClosingSigned_Body send_closing_signed
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.send_closing_signed, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKSendShutdown_Body send_shutdown
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.send_shutdown, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKSendChannelReestablish_Body send_channel_reestablish
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.send_channel_reestablish, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKBroadcastChannelAnnouncement_Body broadcast_channel_announcement
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.broadcast_channel_announcement, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKBroadcastNodeAnnouncement_Body broadcast_node_announcement
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.broadcast_node_announcement, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKBroadcastChannelUpdate_Body broadcast_channel_update
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.broadcast_channel_update, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKHandleError_Body handle_error
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.handle_error, 1));
            }
        }

        public ref LDKMessageSendEvent_LDKPaymentFailureNetworkUpdate_Body payment_failure_network_update
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.payment_failure_network_update, 1));
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKSendAcceptChannel_Body send_accept_channel;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKSendOpenChannel_Body send_open_channel;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKSendFundingCreated_Body send_funding_created;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKSendFundingSigned_Body send_funding_signed;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKSendFundingLocked_Body send_funding_locked;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKSendAnnouncementSignatures_Body send_announcement_signatures;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKUpdateHTLCs_Body update_htl_cs;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKSendRevokeAndACK_Body send_revoke_and_ack;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKSendClosingSigned_Body send_closing_signed;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKSendShutdown_Body send_shutdown;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKSendChannelReestablish_Body send_channel_reestablish;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKBroadcastChannelAnnouncement_Body broadcast_channel_announcement;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKBroadcastNodeAnnouncement_Body broadcast_node_announcement;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKBroadcastChannelUpdate_Body broadcast_channel_update;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKHandleError_Body handle_error;

            [FieldOffset(0)]
            public LDKMessageSendEvent_LDKPaymentFailureNetworkUpdate_Body payment_failure_network_update;
        }
    }

    public unsafe partial struct LDKCVecTempl_MessageSendEvent
    {
        [NativeTypeName("LDKMessageSendEvent *")]
        public LDKMessageSendEvent* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKMessageSendEventsProvider
    {
        [NativeTypeName("void *")]
        public void* this_arg;

        [NativeTypeName("LDKCVec_MessageSendEventZ (*)(const void *)")]
        public IntPtr get_and_clear_pending_msg_events;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr free;
    }

    public unsafe partial struct LDKCVecTempl_Event
    {
        [NativeTypeName("LDKEvent *")]
        public LDKEvent* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKEventsProvider
    {
        [NativeTypeName("void *")]
        public void* this_arg;

        [NativeTypeName("LDKCVec_EventZ (*)(const void *)")]
        public IntPtr get_and_clear_pending_events;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr free;
    }

    public unsafe partial struct LDKLogger
    {
        [NativeTypeName("void *")]
        public void* this_arg;

        [NativeTypeName("void (*)(const void *, const char *)")]
        public IntPtr log;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr free;
    }

    public unsafe partial struct LDKChannelHandshakeConfig
    {
        [NativeTypeName("LDKnativeChannelHandshakeConfig *")]
        public nativeChannelHandshakeConfigOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKChannelHandshakeLimits
    {
        [NativeTypeName("LDKnativeChannelHandshakeLimits *")]
        public nativeChannelHandshakeLimitsOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKChannelConfig
    {
        [NativeTypeName("LDKnativeChannelConfig *")]
        public nativeChannelConfigOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKu8slice
    {
        [NativeTypeName("const uint8_t *")]
        public byte* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKUserConfig
    {
        [NativeTypeName("LDKnativeUserConfig *")]
        public nativeUserConfigOpaque* inner;

        public bool is_owned;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_C2TupleTempl_CVec_u8Z__u64_____ChainError
    {
        [FieldOffset(0)]
        [NativeTypeName("LDKC2TupleTempl_CVec_u8Z__u64 *")]
        public LDKC2TupleTempl_CVec_u8Z__u64* result;

        [FieldOffset(0)]
        [NativeTypeName("LDKChainError *")]
        public LDKChainError* err;
    }

    public partial struct LDKCResultTempl_C2TupleTempl_CVec_u8Z__u64_____ChainError
    {
        public LDKCResultPtr_C2TupleTempl_CVec_u8Z__u64_____ChainError contents;

        public bool result_ok;
    }

    public unsafe partial struct LDKCVecTempl_usize
    {
        [NativeTypeName("uintptr_t *")]
        public UIntPtr* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKChainWatchInterface
    {
        [NativeTypeName("void *")]
        public void* this_arg;

        [NativeTypeName("void (*)(const void *, const uint8_t (*)[32], LDKu8slice)")]
        public IntPtr install_watch_tx;

        [NativeTypeName("void (*)(const void *, LDKC2Tuple_Txidu32Z, LDKu8slice)")]
        public IntPtr install_watch_outpoint;

        [NativeTypeName("void (*)(const void *)")]
        public IntPtr watch_all_txn;

        [NativeTypeName("LDKCResult_C2Tuple_Scriptu64ZChainErrorZ (*)(const void *, LDKThirtyTwoBytes, uint64_t)")]
        public IntPtr get_chain_utxo;

        [NativeTypeName("LDKCVec_usizeZ (*)(const void *, LDKu8slice)")]
        public IntPtr filter_block;

        [NativeTypeName("uintptr_t (*)(const void *)")]
        public IntPtr reentered;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr free;
    }

    public unsafe partial struct LDKTransaction
    {
        [NativeTypeName("const uint8_t *")]
        public byte* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKBroadcasterInterface
    {
        [NativeTypeName("void *")]
        public void* this_arg;

        [NativeTypeName("void (*)(const void *, LDKTransaction)")]
        public IntPtr broadcast_transaction;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr free;
    }

    public unsafe partial struct LDKCVecTempl_CVec_u8Z
    {
        [NativeTypeName("LDKCVec_u8Z *")]
        public LDKCVecTempl_u8* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKusizeslice
    {
        [NativeTypeName("const uintptr_t *")]
        public UIntPtr* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKChainListener
    {
        [NativeTypeName("void *")]
        public void* this_arg;

        [NativeTypeName("void (*)(const void *, const uint8_t (*)[80], uint32_t, LDKCVec_TransactionZ, LDKusizeslice)")]
        public IntPtr block_connected;

        [NativeTypeName("void (*)(const void *, const uint8_t (*)[80], uint32_t)")]
        public IntPtr block_disconnected;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr free;
    }

    public unsafe partial struct LDKFeeEstimator
    {
        [NativeTypeName("void *")]
        public void* this_arg;

        [NativeTypeName("uint32_t (*)(const void *, LDKConfirmationTarget)")]
        public IntPtr get_est_sat_per_1000_weight;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr free;
    }

    public unsafe partial struct LDKChainWatchedUtil
    {
        [NativeTypeName("LDKnativeChainWatchedUtil *")]
        public nativeChainWatchedUtilOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKBlockNotifier
    {
        [NativeTypeName("LDKnativeBlockNotifier *")]
        public nativeBlockNotifierOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKChainWatchInterfaceUtil
    {
        [NativeTypeName("LDKnativeChainWatchInterfaceUtil *")]
        public nativeChainWatchInterfaceUtilOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKChannelPublicKeys
    {
        [NativeTypeName("LDKnativeChannelPublicKeys *")]
        public nativeChannelPublicKeysOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKPreCalculatedTxCreationKeys
    {
        [NativeTypeName("LDKnativePreCalculatedTxCreationKeys *")]
        public nativePreCalculatedTxCreationKeysOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKCVecTempl_HTLCOutputInCommitment
    {
        [NativeTypeName("LDKHTLCOutputInCommitment *")]
        public LDKHTLCOutputInCommitment* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKLocalCommitmentTransaction
    {
        [NativeTypeName("LDKnativeLocalCommitmentTransaction *")]
        public nativeLocalCommitmentTransactionOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKUnsignedChannelAnnouncement
    {
        [NativeTypeName("LDKnativeUnsignedChannelAnnouncement *")]
        public nativeUnsignedChannelAnnouncementOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKSecretKey
    {
        [NativeTypeName("uint8_t [32]")]
        public fixed byte bytes[32];
    }

    public unsafe partial struct LDKKeysInterface
    {
        [NativeTypeName("void *")]
        public void* this_arg;

        [NativeTypeName("LDKSecretKey (*)(const void *)")]
        public IntPtr get_node_secret;

        [NativeTypeName("LDKCVec_u8Z (*)(const void *)")]
        public IntPtr get_destination_script;

        [NativeTypeName("LDKPublicKey (*)(const void *)")]
        public IntPtr get_shutdown_pubkey;

        [NativeTypeName("LDKChannelKeys (*)(const void *, bool, uint64_t)")]
        public IntPtr get_channel_keys;

        [NativeTypeName("LDKThirtyTwoBytes (*)(const void *)")]
        public IntPtr get_secure_random_bytes;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr free;
    }

    public unsafe partial struct LDKInMemoryChannelKeys
    {
        [NativeTypeName("LDKnativeInMemoryChannelKeys *")]
        public nativeInMemoryChannelKeysOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKKeysManager
    {
        [NativeTypeName("LDKnativeKeysManager *")]
        public nativeKeysManagerOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKChannelMonitor
    {
        [NativeTypeName("LDKnativeChannelMonitor *")]
        public nativeChannelMonitorOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKChannelMonitorUpdate
    {
        [NativeTypeName("LDKnativeChannelMonitorUpdate *")]
        public nativeChannelMonitorUpdateOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKMonitorEvent
    {
        [NativeTypeName("LDKnativeMonitorEvent *")]
        public nativeMonitorEventOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKCVecTempl_MonitorEvent
    {
        [NativeTypeName("LDKMonitorEvent *")]
        public LDKMonitorEvent* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKManyChannelMonitor
    {
        [NativeTypeName("void *")]
        public void* this_arg;

        [NativeTypeName("LDKCResult_NoneChannelMonitorUpdateErrZ (*)(const void *, LDKOutPoint, LDKChannelMonitor)")]
        public IntPtr add_monitor;

        [NativeTypeName("LDKCResult_NoneChannelMonitorUpdateErrZ (*)(const void *, LDKOutPoint, LDKChannelMonitorUpdate)")]
        public IntPtr update_monitor;

        [NativeTypeName("LDKCVec_MonitorEventZ (*)(const void *)")]
        public IntPtr get_and_clear_pending_monitor_events;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr free;
    }

    public unsafe partial struct LDKChannelManager
    {
        [NativeTypeName("LDKnativeChannelManager *")]
        public nativeChannelManagerOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKChannelDetails
    {
        [NativeTypeName("LDKnativeChannelDetails *")]
        public nativeChannelDetailsOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKInitFeatures
    {
        [NativeTypeName("LDKnativeInitFeatures *")]
        public nativeInitFeaturesOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKCVecTempl_ChannelDetails
    {
        [NativeTypeName("LDKChannelDetails *")]
        public LDKChannelDetails* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKRoute
    {
        [NativeTypeName("LDKnativeRoute *")]
        public nativeRouteOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKThreeBytes
    {
        [NativeTypeName("uint8_t [3]")]
        public fixed byte data[3];
    }

    public unsafe partial struct LDKFourBytes
    {
        [NativeTypeName("uint8_t [4]")]
        public fixed byte data[4];
    }

    public unsafe partial struct LDKSixteenBytes
    {
        [NativeTypeName("uint8_t [16]")]
        public fixed byte data[16];
    }

    public unsafe partial struct LDKTenBytes
    {
        [NativeTypeName("uint8_t [10]")]
        public fixed byte data[10];
    }

    [NativeTypeName("unsigned int")]
    public enum LDKNetAddress_Tag : uint
    {
        LDKNetAddress_IPv4,
        LDKNetAddress_IPv6,
        LDKNetAddress_OnionV2,
        LDKNetAddress_OnionV3,
        LDKNetAddress_Sentinel,
    }

    public partial struct LDKNetAddress_LDKIPv4_Body
    {
        public LDKFourBytes addr;

        [NativeTypeName("uint16_t")]
        public ushort port;
    }

    public partial struct LDKNetAddress_LDKIPv6_Body
    {
        public LDKSixteenBytes addr;

        [NativeTypeName("uint16_t")]
        public ushort port;
    }

    public partial struct LDKNetAddress_LDKOnionV2_Body
    {
        public LDKTenBytes addr;

        [NativeTypeName("uint16_t")]
        public ushort port;
    }

    public partial struct LDKNetAddress_LDKOnionV3_Body
    {
        public LDKThirtyTwoBytes ed25519_pubkey;

        [NativeTypeName("uint16_t")]
        public ushort checksum;

        [NativeTypeName("uint8_t")]
        public byte version;

        [NativeTypeName("uint16_t")]
        public ushort port;
    }

    public partial struct LDKNetAddress
    {
        public LDKNetAddress_Tag tag;

        [NativeTypeName("LDKNetAddress::(anonymous union at ./rust-lightning/lightning-c-bindings/include/lightning.h:2135:4)")]
        public _Anonymous_e__Union Anonymous;

        public ref LDKNetAddress_LDKIPv4_Body i_pv4
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.i_pv4, 1));
            }
        }

        public ref LDKNetAddress_LDKIPv6_Body i_pv6
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.i_pv6, 1));
            }
        }

        public ref LDKNetAddress_LDKOnionV2_Body onion_v2
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.onion_v2, 1));
            }
        }

        public ref LDKNetAddress_LDKOnionV3_Body onion_v3
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.onion_v3, 1));
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public LDKNetAddress_LDKIPv4_Body i_pv4;

            [FieldOffset(0)]
            public LDKNetAddress_LDKIPv6_Body i_pv6;

            [FieldOffset(0)]
            public LDKNetAddress_LDKOnionV2_Body onion_v2;

            [FieldOffset(0)]
            public LDKNetAddress_LDKOnionV3_Body onion_v3;
        }
    }

    public unsafe partial struct LDKCVecTempl_NetAddress
    {
        [NativeTypeName("LDKNetAddress *")]
        public LDKNetAddress* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKUpdateAddHTLC
    {
        [NativeTypeName("LDKnativeUpdateAddHTLC *")]
        public nativeUpdateAddHTLCOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKUpdateFulfillHTLC
    {
        [NativeTypeName("LDKnativeUpdateFulfillHTLC *")]
        public nativeUpdateFulfillHTLCOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKUpdateFailHTLC
    {
        [NativeTypeName("LDKnativeUpdateFailHTLC *")]
        public nativeUpdateFailHTLCOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKUpdateFailMalformedHTLC
    {
        [NativeTypeName("LDKnativeUpdateFailMalformedHTLC *")]
        public nativeUpdateFailMalformedHTLCOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKCommitmentSigned
    {
        [NativeTypeName("LDKnativeCommitmentSigned *")]
        public nativeCommitmentSignedOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKUpdateFee
    {
        [NativeTypeName("LDKnativeUpdateFee *")]
        public nativeUpdateFeeOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKInit
    {
        [NativeTypeName("LDKnativeInit *")]
        public nativeInitOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKChannelMessageHandler
    {
        [NativeTypeName("void *")]
        public void* this_arg;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, LDKInitFeatures, const LDKOpenChannel *)")]
        public IntPtr handle_open_channel;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, LDKInitFeatures, const LDKAcceptChannel *)")]
        public IntPtr handle_accept_channel;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKFundingCreated *)")]
        public IntPtr handle_funding_created;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKFundingSigned *)")]
        public IntPtr handle_funding_signed;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKFundingLocked *)")]
        public IntPtr handle_funding_locked;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKShutdown *)")]
        public IntPtr handle_shutdown;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKClosingSigned *)")]
        public IntPtr handle_closing_signed;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKUpdateAddHTLC *)")]
        public IntPtr handle_update_add_htlc;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKUpdateFulfillHTLC *)")]
        public IntPtr handle_update_fulfill_htlc;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKUpdateFailHTLC *)")]
        public IntPtr handle_update_fail_htlc;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKUpdateFailMalformedHTLC *)")]
        public IntPtr handle_update_fail_malformed_htlc;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKCommitmentSigned *)")]
        public IntPtr handle_commitment_signed;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKRevokeAndACK *)")]
        public IntPtr handle_revoke_and_ack;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKUpdateFee *)")]
        public IntPtr handle_update_fee;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKAnnouncementSignatures *)")]
        public IntPtr handle_announcement_signatures;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, bool)")]
        public IntPtr peer_disconnected;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKInit *)")]
        public IntPtr peer_connected;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKChannelReestablish *)")]
        public IntPtr handle_channel_reestablish;

        [NativeTypeName("void (*)(const void *, LDKPublicKey, const LDKErrorMessage *)")]
        public IntPtr handle_error;

        public LDKMessageSendEventsProvider MessageSendEventsProvider;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr free;
    }

    public unsafe partial struct LDKChannelManagerReadArgs
    {
        [NativeTypeName("LDKnativeChannelManagerReadArgs *")]
        public nativeChannelManagerReadArgsOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKCVecTempl_ChannelMonitor
    {
        [NativeTypeName("LDKChannelMonitor *")]
        public LDKChannelMonitor* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKHTLCUpdate
    {
        [NativeTypeName("LDKnativeHTLCUpdate *")]
        public nativeHTLCUpdateOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKDecodeError
    {
        [NativeTypeName("LDKnativeDecodeError *")]
        public nativeDecodeErrorOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKPing
    {
        [NativeTypeName("LDKnativePing *")]
        public nativePingOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKPong
    {
        [NativeTypeName("LDKnativePong *")]
        public nativePongOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKDataLossProtect
    {
        [NativeTypeName("LDKnativeDataLossProtect *")]
        public nativeDataLossProtectOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKUnsignedNodeAnnouncement
    {
        [NativeTypeName("LDKnativeUnsignedNodeAnnouncement *")]
        public nativeUnsignedNodeAnnouncementOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKUnsignedChannelUpdate
    {
        [NativeTypeName("LDKnativeUnsignedChannelUpdate *")]
        public nativeUnsignedChannelUpdateOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKLightningError
    {
        [NativeTypeName("LDKnativeLightningError *")]
        public nativeLightningErrorOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKCVecTempl_UpdateAddHTLC
    {
        [NativeTypeName("LDKUpdateAddHTLC *")]
        public LDKUpdateAddHTLC* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKCVecTempl_UpdateFulfillHTLC
    {
        [NativeTypeName("LDKUpdateFulfillHTLC *")]
        public LDKUpdateFulfillHTLC* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKCVecTempl_UpdateFailHTLC
    {
        [NativeTypeName("LDKUpdateFailHTLC *")]
        public LDKUpdateFailHTLC* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKCVecTempl_UpdateFailMalformedHTLC
    {
        [NativeTypeName("LDKUpdateFailMalformedHTLC *")]
        public LDKUpdateFailMalformedHTLC* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_bool__LightningError
    {
        [FieldOffset(0)]
        [NativeTypeName("bool *")]
        public bool* result;

        [FieldOffset(0)]
        [NativeTypeName("LDKLightningError *")]
        public LDKLightningError* err;
    }

    public partial struct LDKCResultTempl_bool__LightningError
    {
        public LDKCResultPtr_bool__LightningError contents;

        public bool result_ok;
    }

    public unsafe partial struct LDKCVecTempl_C3TupleTempl_ChannelAnnouncement__ChannelUpdate__ChannelUpdate
    {
        [NativeTypeName("LDKC3TupleTempl_ChannelAnnouncement__ChannelUpdate__ChannelUpdate *")]
        public LDKC3TupleTempl_ChannelAnnouncement__ChannelUpdate__ChannelUpdate* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKCVecTempl_NodeAnnouncement
    {
        [NativeTypeName("LDKNodeAnnouncement *")]
        public LDKNodeAnnouncement* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKRoutingMessageHandler
    {
        [NativeTypeName("void *")]
        public void* this_arg;

        [NativeTypeName("LDKCResult_boolLightningErrorZ (*)(const void *, const LDKNodeAnnouncement *)")]
        public IntPtr handle_node_announcement;

        [NativeTypeName("LDKCResult_boolLightningErrorZ (*)(const void *, const LDKChannelAnnouncement *)")]
        public IntPtr handle_channel_announcement;

        [NativeTypeName("LDKCResult_boolLightningErrorZ (*)(const void *, const LDKChannelUpdate *)")]
        public IntPtr handle_channel_update;

        [NativeTypeName("void (*)(const void *, const LDKHTLCFailChannelUpdate *)")]
        public IntPtr handle_htlc_fail_channel_update;

        [NativeTypeName("LDKCVec_C3Tuple_ChannelAnnouncementChannelUpdateChannelUpdateZZ (*)(const void *, uint64_t, uint8_t)")]
        public IntPtr get_next_channel_announcements;

        [NativeTypeName("LDKCVec_NodeAnnouncementZ (*)(const void *, LDKPublicKey, uint8_t)")]
        public IntPtr get_next_node_announcements;

        [NativeTypeName("bool (*)(const void *, LDKPublicKey)")]
        public IntPtr should_request_full_sync;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr free;
    }

    public unsafe partial struct LDKMessageHandler
    {
        [NativeTypeName("LDKnativeMessageHandler *")]
        public nativeMessageHandlerOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKSocketDescriptor
    {
        [NativeTypeName("void *")]
        public void* this_arg;

        [NativeTypeName("uintptr_t (*)(void *, LDKu8slice, bool)")]
        public IntPtr send_data;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr disconnect_socket;

        [NativeTypeName("bool (*)(const void *, const void *)")]
        public IntPtr eq;

        [NativeTypeName("uint64_t (*)(const void *)")]
        public IntPtr hash;

        [NativeTypeName("void *(*)(const void *)")]
        public IntPtr clone;

        [NativeTypeName("void (*)(void *)")]
        public IntPtr free;
    }

    public unsafe partial struct LDKPeerManager
    {
        [NativeTypeName("LDKnativePeerManager *")]
        public nativePeerManagerOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKCVecTempl_PublicKey
    {
        [NativeTypeName("LDKPublicKey *")]
        public LDKPublicKey* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_CVecTempl_u8_____PeerHandleError
    {
        [FieldOffset(0)]
        [NativeTypeName("LDKCVecTempl_u8 *")]
        public LDKCVecTempl_u8* result;

        [FieldOffset(0)]
        [NativeTypeName("LDKPeerHandleError *")]
        public LDKPeerHandleError* err;
    }

    public partial struct LDKCResultTempl_CVecTempl_u8_____PeerHandleError
    {
        public LDKCResultPtr_CVecTempl_u8_____PeerHandleError contents;

        public bool result_ok;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_bool__PeerHandleError
    {
        [FieldOffset(0)]
        [NativeTypeName("bool *")]
        public bool* result;

        [FieldOffset(0)]
        [NativeTypeName("LDKPeerHandleError *")]
        public LDKPeerHandleError* err;
    }

    public partial struct LDKCResultTempl_bool__PeerHandleError
    {
        public LDKCResultPtr_bool__PeerHandleError contents;

        public bool result_ok;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_SecretKey__Secp256k1Error
    {
        [FieldOffset(0)]
        [NativeTypeName("LDKSecretKey *")]
        public LDKSecretKey* result;

        [FieldOffset(0)]
        [NativeTypeName("LDKSecp256k1Error *")]
        public LDKSecp256k1Error* err;
    }

    public partial struct LDKCResultTempl_SecretKey__Secp256k1Error
    {
        public LDKCResultPtr_SecretKey__Secp256k1Error contents;

        public bool result_ok;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_PublicKey__Secp256k1Error
    {
        [FieldOffset(0)]
        [NativeTypeName("LDKPublicKey *")]
        public LDKPublicKey* result;

        [FieldOffset(0)]
        [NativeTypeName("LDKSecp256k1Error *")]
        public LDKSecp256k1Error* err;
    }

    public partial struct LDKCResultTempl_PublicKey__Secp256k1Error
    {
        public LDKCResultPtr_PublicKey__Secp256k1Error contents;

        public bool result_ok;
    }

    public unsafe partial struct LDKTxCreationKeys
    {
        [NativeTypeName("LDKnativeTxCreationKeys *")]
        public nativeTxCreationKeysOpaque* inner;

        public bool is_owned;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_TxCreationKeys__Secp256k1Error
    {
        [FieldOffset(0)]
        [NativeTypeName("LDKTxCreationKeys *")]
        public LDKTxCreationKeys* result;

        [FieldOffset(0)]
        [NativeTypeName("LDKSecp256k1Error *")]
        public LDKSecp256k1Error* err;
    }

    public partial struct LDKCResultTempl_TxCreationKeys__Secp256k1Error
    {
        public LDKCResultPtr_TxCreationKeys__Secp256k1Error contents;

        public bool result_ok;
    }

    public unsafe partial struct LDKCVecTempl_C2TupleTempl_HTLCOutputInCommitment__Signature
    {
        [NativeTypeName("LDKC2TupleTempl_HTLCOutputInCommitment__Signature *")]
        public LDKC2TupleTempl_HTLCOutputInCommitment__Signature* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKNodeFeatures
    {
        [NativeTypeName("LDKnativeNodeFeatures *")]
        public nativeNodeFeaturesOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKChannelFeatures
    {
        [NativeTypeName("LDKnativeChannelFeatures *")]
        public nativeChannelFeaturesOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKRouteHop
    {
        [NativeTypeName("LDKnativeRouteHop *")]
        public nativeRouteHopOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKCVecTempl_RouteHop
    {
        [NativeTypeName("LDKRouteHop *")]
        public LDKRouteHop* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKCVecTempl_CVecTempl_RouteHop
    {
        [NativeTypeName("LDKCVecTempl_RouteHop *")]
        public LDKCVecTempl_RouteHop* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKRouteHint
    {
        [NativeTypeName("LDKnativeRouteHint *")]
        public nativeRouteHintOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKRoutingFees
    {
        [NativeTypeName("LDKnativeRoutingFees *")]
        public nativeRoutingFeesOpaque* inner;

        public bool is_owned;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct LDKCResultPtr_Route__LightningError
    {
        [FieldOffset(0)]
        [NativeTypeName("LDKRoute *")]
        public LDKRoute* result;

        [FieldOffset(0)]
        [NativeTypeName("LDKLightningError *")]
        public LDKLightningError* err;
    }

    public partial struct LDKCResultTempl_Route__LightningError
    {
        public LDKCResultPtr_Route__LightningError contents;

        public bool result_ok;
    }

    public unsafe partial struct LDKNetworkGraph
    {
        [NativeTypeName("LDKnativeNetworkGraph *")]
        public nativeNetworkGraphOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKCVecTempl_RouteHint
    {
        [NativeTypeName("LDKRouteHint *")]
        public LDKRouteHint* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public unsafe partial struct LDKLockedNetworkGraph
    {
        [NativeTypeName("LDKnativeLockedNetworkGraph *")]
        public nativeLockedNetworkGraphOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKNetGraphMsgHandler
    {
        [NativeTypeName("LDKnativeNetGraphMsgHandler *")]
        public nativeNetGraphMsgHandlerOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKDirectionalChannelInfo
    {
        [NativeTypeName("LDKnativeDirectionalChannelInfo *")]
        public nativeDirectionalChannelInfoOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKChannelInfo
    {
        [NativeTypeName("LDKnativeChannelInfo *")]
        public nativeChannelInfoOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKNodeAnnouncementInfo
    {
        [NativeTypeName("LDKnativeNodeAnnouncementInfo *")]
        public nativeNodeAnnouncementInfoOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKNodeInfo
    {
        [NativeTypeName("LDKnativeNodeInfo *")]
        public nativeNodeInfoOpaque* inner;

        public bool is_owned;
    }

    public unsafe partial struct LDKCVecTempl_u64
    {
        [NativeTypeName("uint64_t *")]
        public ulong* data;

        [NativeTypeName("uintptr_t")]
        public UIntPtr datalen;
    }

    public static unsafe partial class Methods
    {
        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void TxOut_free(LDKTxOut _res);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKC2Tuple_Txidu32Z")]
        public static extern LDKC2TupleTempl_ThirtyTwoBytes__u32 C2Tuple_Txidu32Z_new(LDKThirtyTwoBytes a, [NativeTypeName("uint32_t")] uint b);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKC2Tuple_Scriptu64Z")]
        public static extern LDKC2TupleTempl_CVec_u8Z__u64 C2Tuple_Scriptu64Z_new([NativeTypeName("LDKCVec_u8Z")] LDKCVecTempl_u8 a, [NativeTypeName("uint64_t")] ulong b);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKC2Tuple_u64u64Z")]
        public static extern LDKC2TupleTempl_u64__u64 C2Tuple_u64u64Z_new([NativeTypeName("uint64_t")] ulong a, [NativeTypeName("uint64_t")] ulong b);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKC2Tuple_SignatureCVec_SignatureZZ")]
        public static extern LDKC2TupleTempl_Signature__CVecTempl_Signature C2Tuple_SignatureCVec_SignatureZZ_new(LDKSignature a, [NativeTypeName("LDKCVec_SignatureZ")] LDKCVecTempl_Signature b);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_C2Tuple_SignatureCVec_SignatureZZNoneZ")]
        public static extern LDKCResultTempl_C2TupleTempl_Signature__CVecTempl_Signature________u8 CResult_C2Tuple_SignatureCVec_SignatureZZNoneZ_err();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_SignatureNoneZ")]
        public static extern LDKCResultTempl_Signature__u8 CResult_SignatureNoneZ_err();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_CVec_SignatureZNoneZ")]
        public static extern LDKCResultTempl_CVecTempl_Signature_____u8 CResult_CVec_SignatureZNoneZ_err();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_NoneAPIErrorZ")]
        public static extern LDKCResultTempl_u8__APIError CResult_NoneAPIErrorZ_ok();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_NonePaymentSendFailureZ")]
        public static extern LDKCResultTempl_u8__PaymentSendFailure CResult_NonePaymentSendFailureZ_ok();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_NoneChannelMonitorUpdateErrZ")]
        public static extern LDKCResultTempl_u8__ChannelMonitorUpdateErr CResult_NoneChannelMonitorUpdateErrZ_ok();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_NoneMonitorUpdateErrorZ")]
        public static extern LDKCResultTempl_u8__MonitorUpdateError CResult_NoneMonitorUpdateErrorZ_ok();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKC2Tuple_OutPointScriptZ")]
        public static extern LDKC2TupleTempl_OutPoint__CVec_u8Z C2Tuple_OutPointScriptZ_new(LDKOutPoint a, [NativeTypeName("LDKCVec_u8Z")] LDKCVecTempl_u8 b);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKC3Tuple_ChannelAnnouncementChannelUpdateChannelUpdateZ")]
        public static extern LDKC3TupleTempl_ChannelAnnouncement__ChannelUpdate__ChannelUpdate C3Tuple_ChannelAnnouncementChannelUpdateChannelUpdateZ_new(LDKChannelAnnouncement a, LDKChannelUpdate b, LDKChannelUpdate c);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_NonePeerHandleErrorZ")]
        public static extern LDKCResultTempl_u8__PeerHandleError CResult_NonePeerHandleErrorZ_ok();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKC2Tuple_HTLCOutputInCommitmentSignatureZ")]
        public static extern LDKC2TupleTempl_HTLCOutputInCommitment__Signature C2Tuple_HTLCOutputInCommitmentSignatureZ_new(LDKHTLCOutputInCommitment a, LDKSignature b);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Event_free(LDKEvent this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MessageSendEvent_free(LDKMessageSendEvent this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MessageSendEventsProvider_free(LDKMessageSendEventsProvider this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void EventsProvider_free(LDKEventsProvider this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void APIError_free(LDKAPIError this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKLevel Level_max();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Logger_free(LDKLogger this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeConfig_free(LDKChannelHandshakeConfig this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint ChannelHandshakeConfig_get_minimum_depth([NativeTypeName("const LDKChannelHandshakeConfig *")] LDKChannelHandshakeConfig* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeConfig_set_minimum_depth([NativeTypeName("LDKChannelHandshakeConfig *")] LDKChannelHandshakeConfig* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort ChannelHandshakeConfig_get_our_to_self_delay([NativeTypeName("const LDKChannelHandshakeConfig *")] LDKChannelHandshakeConfig* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeConfig_set_our_to_self_delay([NativeTypeName("LDKChannelHandshakeConfig *")] LDKChannelHandshakeConfig* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelHandshakeConfig_get_our_htlc_minimum_msat([NativeTypeName("const LDKChannelHandshakeConfig *")] LDKChannelHandshakeConfig* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeConfig_set_our_htlc_minimum_msat([NativeTypeName("LDKChannelHandshakeConfig *")] LDKChannelHandshakeConfig* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelHandshakeConfig ChannelHandshakeConfig_new([NativeTypeName("uint32_t")] uint minimum_depth_arg, [NativeTypeName("uint16_t")] ushort our_to_self_delay_arg, [NativeTypeName("uint64_t")] ulong our_htlc_minimum_msat_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelHandshakeConfig ChannelHandshakeConfig_default();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeLimits_free(LDKChannelHandshakeLimits this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelHandshakeLimits_get_min_funding_satoshis([NativeTypeName("const LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeLimits_set_min_funding_satoshis([NativeTypeName("LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelHandshakeLimits_get_max_htlc_minimum_msat([NativeTypeName("const LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeLimits_set_max_htlc_minimum_msat([NativeTypeName("LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelHandshakeLimits_get_min_max_htlc_value_in_flight_msat([NativeTypeName("const LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeLimits_set_min_max_htlc_value_in_flight_msat([NativeTypeName("LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelHandshakeLimits_get_max_channel_reserve_satoshis([NativeTypeName("const LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeLimits_set_max_channel_reserve_satoshis([NativeTypeName("LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort ChannelHandshakeLimits_get_min_max_accepted_htlcs([NativeTypeName("const LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeLimits_set_min_max_accepted_htlcs([NativeTypeName("LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelHandshakeLimits_get_min_dust_limit_satoshis([NativeTypeName("const LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeLimits_set_min_dust_limit_satoshis([NativeTypeName("LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelHandshakeLimits_get_max_dust_limit_satoshis([NativeTypeName("const LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeLimits_set_max_dust_limit_satoshis([NativeTypeName("LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint ChannelHandshakeLimits_get_max_minimum_depth([NativeTypeName("const LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeLimits_set_max_minimum_depth([NativeTypeName("LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte ChannelHandshakeLimits_get_force_announced_channel_preference([NativeTypeName("const LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeLimits_set_force_announced_channel_preference([NativeTypeName("LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr, [NativeTypeName("bool")] byte val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort ChannelHandshakeLimits_get_their_to_self_delay([NativeTypeName("const LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelHandshakeLimits_set_their_to_self_delay([NativeTypeName("LDKChannelHandshakeLimits *")] LDKChannelHandshakeLimits* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelHandshakeLimits ChannelHandshakeLimits_new([NativeTypeName("uint64_t")] ulong min_funding_satoshis_arg, [NativeTypeName("uint64_t")] ulong max_htlc_minimum_msat_arg, [NativeTypeName("uint64_t")] ulong min_max_htlc_value_in_flight_msat_arg, [NativeTypeName("uint64_t")] ulong max_channel_reserve_satoshis_arg, [NativeTypeName("uint16_t")] ushort min_max_accepted_htlcs_arg, [NativeTypeName("uint64_t")] ulong min_dust_limit_satoshis_arg, [NativeTypeName("uint64_t")] ulong max_dust_limit_satoshis_arg, [NativeTypeName("uint32_t")] uint max_minimum_depth_arg, [NativeTypeName("bool")] byte force_announced_channel_preference_arg, [NativeTypeName("uint16_t")] ushort their_to_self_delay_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelHandshakeLimits ChannelHandshakeLimits_default();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelConfig_free(LDKChannelConfig this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint ChannelConfig_get_fee_proportional_millionths([NativeTypeName("const LDKChannelConfig *")] LDKChannelConfig* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelConfig_set_fee_proportional_millionths([NativeTypeName("LDKChannelConfig *")] LDKChannelConfig* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte ChannelConfig_get_announced_channel([NativeTypeName("const LDKChannelConfig *")] LDKChannelConfig* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelConfig_set_announced_channel([NativeTypeName("LDKChannelConfig *")] LDKChannelConfig* this_ptr, [NativeTypeName("bool")] byte val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte ChannelConfig_get_commit_upfront_shutdown_pubkey([NativeTypeName("const LDKChannelConfig *")] LDKChannelConfig* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelConfig_set_commit_upfront_shutdown_pubkey([NativeTypeName("LDKChannelConfig *")] LDKChannelConfig* this_ptr, [NativeTypeName("bool")] byte val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelConfig ChannelConfig_new([NativeTypeName("uint32_t")] uint fee_proportional_millionths_arg, [NativeTypeName("bool")] byte announced_channel_arg, [NativeTypeName("bool")] byte commit_upfront_shutdown_pubkey_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelConfig ChannelConfig_default();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 ChannelConfig_write([NativeTypeName("const LDKChannelConfig *")] LDKChannelConfig* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelConfig ChannelConfig_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UserConfig_free(LDKUserConfig this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelHandshakeConfig UserConfig_get_own_channel_config([NativeTypeName("const LDKUserConfig *")] LDKUserConfig* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UserConfig_set_own_channel_config([NativeTypeName("LDKUserConfig *")] LDKUserConfig* this_ptr, LDKChannelHandshakeConfig val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelHandshakeLimits UserConfig_get_peer_channel_config_limits([NativeTypeName("const LDKUserConfig *")] LDKUserConfig* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UserConfig_set_peer_channel_config_limits([NativeTypeName("LDKUserConfig *")] LDKUserConfig* this_ptr, LDKChannelHandshakeLimits val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelConfig UserConfig_get_channel_options([NativeTypeName("const LDKUserConfig *")] LDKUserConfig* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UserConfig_set_channel_options([NativeTypeName("LDKUserConfig *")] LDKUserConfig* this_ptr, LDKChannelConfig val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUserConfig UserConfig_new(LDKChannelHandshakeConfig own_channel_config_arg, LDKChannelHandshakeLimits peer_channel_config_limits_arg, LDKChannelConfig channel_options_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUserConfig UserConfig_default();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChainWatchInterface_free(LDKChainWatchInterface this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void BroadcasterInterface_free(LDKBroadcasterInterface this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChainListener_free(LDKChainListener this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void FeeEstimator_free(LDKFeeEstimator this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChainWatchedUtil_free(LDKChainWatchedUtil this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChainWatchedUtil ChainWatchedUtil_new();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte ChainWatchedUtil_register_tx([NativeTypeName("LDKChainWatchedUtil *")] LDKChainWatchedUtil* this_arg, [NativeTypeName("const uint8_t (*)[32]")] byte** txid, LDKu8slice script_pub_key);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte ChainWatchedUtil_register_outpoint([NativeTypeName("LDKChainWatchedUtil *")] LDKChainWatchedUtil* this_arg, [NativeTypeName("LDKC2Tuple_Txidu32Z")] LDKC2TupleTempl_ThirtyTwoBytes__u32 outpoint, LDKu8slice _script_pub_key);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte ChainWatchedUtil_watch_all([NativeTypeName("LDKChainWatchedUtil *")] LDKChainWatchedUtil* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte ChainWatchedUtil_does_match_tx([NativeTypeName("const LDKChainWatchedUtil *")] LDKChainWatchedUtil* this_arg, LDKTransaction tx);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void BlockNotifier_free(LDKBlockNotifier this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKBlockNotifier BlockNotifier_new(LDKChainWatchInterface chain_monitor);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void BlockNotifier_register_listener([NativeTypeName("const LDKBlockNotifier *")] LDKBlockNotifier* this_arg, LDKChainListener listener);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void BlockNotifier_block_connected([NativeTypeName("const LDKBlockNotifier *")] LDKBlockNotifier* this_arg, LDKu8slice block, [NativeTypeName("uint32_t")] uint height);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte BlockNotifier_block_connected_checked([NativeTypeName("const LDKBlockNotifier *")] LDKBlockNotifier* this_arg, [NativeTypeName("const uint8_t (*)[80]")] byte** header, [NativeTypeName("uint32_t")] uint height, [NativeTypeName("LDKCVec_TransactionZ")] LDKCVecTempl_CVec_u8Z txn_matched, LDKusizeslice indexes_of_txn_matched);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void BlockNotifier_block_disconnected([NativeTypeName("const LDKBlockNotifier *")] LDKBlockNotifier* this_arg, [NativeTypeName("const uint8_t (*)[80]")] byte** header, [NativeTypeName("uint32_t")] uint disconnected_height);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChainWatchInterfaceUtil_free(LDKChainWatchInterfaceUtil this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChainWatchInterface ChainWatchInterfaceUtil_as_ChainWatchInterface([NativeTypeName("const LDKChainWatchInterfaceUtil *")] LDKChainWatchInterfaceUtil* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChainWatchInterfaceUtil ChainWatchInterfaceUtil_new(LDKNetwork network);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte ChainWatchInterfaceUtil_does_match_tx([NativeTypeName("const LDKChainWatchInterfaceUtil *")] LDKChainWatchInterfaceUtil* this_arg, LDKTransaction tx);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OutPoint_free(LDKOutPoint this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** OutPoint_get_txid([NativeTypeName("const LDKOutPoint *")] LDKOutPoint* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OutPoint_set_txid([NativeTypeName("LDKOutPoint *")] LDKOutPoint* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort OutPoint_get_index([NativeTypeName("const LDKOutPoint *")] LDKOutPoint* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OutPoint_set_index([NativeTypeName("LDKOutPoint *")] LDKOutPoint* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKOutPoint OutPoint_new(LDKThirtyTwoBytes txid_arg, [NativeTypeName("uint16_t")] ushort index_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKThirtyTwoBytes OutPoint_to_channel_id([NativeTypeName("const LDKOutPoint *")] LDKOutPoint* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 OutPoint_write([NativeTypeName("const LDKOutPoint *")] LDKOutPoint* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKOutPoint OutPoint_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void SpendableOutputDescriptor_free(LDKSpendableOutputDescriptor this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelKeys_free(LDKChannelKeys this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void KeysInterface_free(LDKKeysInterface this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void InMemoryChannelKeys_free(LDKInMemoryChannelKeys this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** InMemoryChannelKeys_get_funding_key([NativeTypeName("const LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void InMemoryChannelKeys_set_funding_key([NativeTypeName("LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_ptr, LDKSecretKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** InMemoryChannelKeys_get_revocation_base_key([NativeTypeName("const LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void InMemoryChannelKeys_set_revocation_base_key([NativeTypeName("LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_ptr, LDKSecretKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** InMemoryChannelKeys_get_payment_key([NativeTypeName("const LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void InMemoryChannelKeys_set_payment_key([NativeTypeName("LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_ptr, LDKSecretKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** InMemoryChannelKeys_get_delayed_payment_base_key([NativeTypeName("const LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void InMemoryChannelKeys_set_delayed_payment_base_key([NativeTypeName("LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_ptr, LDKSecretKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** InMemoryChannelKeys_get_htlc_base_key([NativeTypeName("const LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void InMemoryChannelKeys_set_htlc_base_key([NativeTypeName("LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_ptr, LDKSecretKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** InMemoryChannelKeys_get_commitment_seed([NativeTypeName("const LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void InMemoryChannelKeys_set_commitment_seed([NativeTypeName("LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKInMemoryChannelKeys InMemoryChannelKeys_new(LDKSecretKey funding_key, LDKSecretKey revocation_base_key, LDKSecretKey payment_key, LDKSecretKey delayed_payment_base_key, LDKSecretKey htlc_base_key, LDKThirtyTwoBytes commitment_seed, [NativeTypeName("uint64_t")] ulong channel_value_satoshis, [NativeTypeName("LDKC2Tuple_u64u64Z")] LDKC2TupleTempl_u64__u64 key_derivation_params);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelPublicKeys InMemoryChannelKeys_remote_pubkeys([NativeTypeName("const LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort InMemoryChannelKeys_remote_to_self_delay([NativeTypeName("const LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort InMemoryChannelKeys_local_to_self_delay([NativeTypeName("const LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelKeys InMemoryChannelKeys_as_ChannelKeys([NativeTypeName("const LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 InMemoryChannelKeys_write([NativeTypeName("const LDKInMemoryChannelKeys *")] LDKInMemoryChannelKeys* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKInMemoryChannelKeys InMemoryChannelKeys_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void KeysManager_free(LDKKeysManager this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKKeysManager KeysManager_new([NativeTypeName("const uint8_t (*)[32]")] byte** seed, LDKNetwork network, [NativeTypeName("uint64_t")] ulong starting_time_secs, [NativeTypeName("uint32_t")] uint starting_time_nanos);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKInMemoryChannelKeys KeysManager_derive_channel_keys([NativeTypeName("const LDKKeysManager *")] LDKKeysManager* this_arg, [NativeTypeName("uint64_t")] ulong channel_value_satoshis, [NativeTypeName("uint64_t")] ulong params_1, [NativeTypeName("uint64_t")] ulong params_2);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKKeysInterface KeysManager_as_KeysInterface([NativeTypeName("const LDKKeysManager *")] LDKKeysManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManager_free(LDKChannelManager this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelDetails_free(LDKChannelDetails this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** ChannelDetails_get_channel_id([NativeTypeName("const LDKChannelDetails *")] LDKChannelDetails* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelDetails_set_channel_id([NativeTypeName("LDKChannelDetails *")] LDKChannelDetails* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey ChannelDetails_get_remote_network_id([NativeTypeName("const LDKChannelDetails *")] LDKChannelDetails* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelDetails_set_remote_network_id([NativeTypeName("LDKChannelDetails *")] LDKChannelDetails* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKInitFeatures ChannelDetails_get_counterparty_features([NativeTypeName("const LDKChannelDetails *")] LDKChannelDetails* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelDetails_set_counterparty_features([NativeTypeName("LDKChannelDetails *")] LDKChannelDetails* this_ptr, LDKInitFeatures val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelDetails_get_channel_value_satoshis([NativeTypeName("const LDKChannelDetails *")] LDKChannelDetails* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelDetails_set_channel_value_satoshis([NativeTypeName("LDKChannelDetails *")] LDKChannelDetails* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelDetails_get_user_id([NativeTypeName("const LDKChannelDetails *")] LDKChannelDetails* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelDetails_set_user_id([NativeTypeName("LDKChannelDetails *")] LDKChannelDetails* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelDetails_get_outbound_capacity_msat([NativeTypeName("const LDKChannelDetails *")] LDKChannelDetails* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelDetails_set_outbound_capacity_msat([NativeTypeName("LDKChannelDetails *")] LDKChannelDetails* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelDetails_get_inbound_capacity_msat([NativeTypeName("const LDKChannelDetails *")] LDKChannelDetails* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelDetails_set_inbound_capacity_msat([NativeTypeName("LDKChannelDetails *")] LDKChannelDetails* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte ChannelDetails_get_is_live([NativeTypeName("const LDKChannelDetails *")] LDKChannelDetails* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelDetails_set_is_live([NativeTypeName("LDKChannelDetails *")] LDKChannelDetails* this_ptr, [NativeTypeName("bool")] byte val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void PaymentSendFailure_free(LDKPaymentSendFailure this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelManager ChannelManager_new(LDKNetwork network, LDKFeeEstimator fee_est, LDKManyChannelMonitor monitor, LDKBroadcasterInterface tx_broadcaster, LDKLogger logger, LDKKeysInterface keys_manager, LDKUserConfig config, [NativeTypeName("uintptr_t")] UIntPtr current_blockchain_height);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_NoneAPIErrorZ")]
        public static extern LDKCResultTempl_u8__APIError ChannelManager_create_channel([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg, LDKPublicKey their_network_key, [NativeTypeName("uint64_t")] ulong channel_value_satoshis, [NativeTypeName("uint64_t")] ulong push_msat, [NativeTypeName("uint64_t")] ulong user_id, LDKUserConfig override_config);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_ChannelDetailsZ")]
        public static extern LDKCVecTempl_ChannelDetails ChannelManager_list_channels([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_ChannelDetailsZ")]
        public static extern LDKCVecTempl_ChannelDetails ChannelManager_list_usable_channels([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_NoneAPIErrorZ")]
        public static extern LDKCResultTempl_u8__APIError ChannelManager_close_channel([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg, [NativeTypeName("const uint8_t (*)[32]")] byte** channel_id);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManager_force_close_channel([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg, [NativeTypeName("const uint8_t (*)[32]")] byte** channel_id);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManager_force_close_all_channels([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_NonePaymentSendFailureZ")]
        public static extern LDKCResultTempl_u8__PaymentSendFailure ChannelManager_send_payment([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg, [NativeTypeName("const LDKRoute *")] LDKRoute* route, LDKThirtyTwoBytes payment_hash, LDKThirtyTwoBytes payment_secret);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManager_funding_transaction_generated([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg, [NativeTypeName("const uint8_t (*)[32]")] byte** temporary_channel_id, LDKOutPoint funding_txo);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManager_broadcast_node_announcement([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg, LDKThreeBytes rgb, LDKThirtyTwoBytes alias, [NativeTypeName("LDKCVec_NetAddressZ")] LDKCVecTempl_NetAddress addresses);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManager_process_pending_htlc_forwards([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManager_timer_chan_freshness_every_min([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte ChannelManager_fail_htlc_backwards([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg, [NativeTypeName("const uint8_t (*)[32]")] byte** payment_hash, LDKThirtyTwoBytes payment_secret);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte ChannelManager_claim_funds([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg, LDKThirtyTwoBytes payment_preimage, LDKThirtyTwoBytes payment_secret, [NativeTypeName("uint64_t")] ulong expected_amount);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey ChannelManager_get_our_node_id([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManager_channel_monitor_updated([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg, [NativeTypeName("const LDKOutPoint *")] LDKOutPoint* funding_txo, [NativeTypeName("uint64_t")] ulong highest_applied_update_id);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKMessageSendEventsProvider ChannelManager_as_MessageSendEventsProvider([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKEventsProvider ChannelManager_as_EventsProvider([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChainListener ChannelManager_as_ChainListener([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelMessageHandler ChannelManager_as_ChannelMessageHandler([NativeTypeName("const LDKChannelManager *")] LDKChannelManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManagerReadArgs_free(LDKChannelManagerReadArgs this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const LDKKeysInterface *")]
        public static extern LDKKeysInterface* ChannelManagerReadArgs_get_keys_manager([NativeTypeName("const LDKChannelManagerReadArgs *")] LDKChannelManagerReadArgs* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManagerReadArgs_set_keys_manager([NativeTypeName("LDKChannelManagerReadArgs *")] LDKChannelManagerReadArgs* this_ptr, LDKKeysInterface val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const LDKFeeEstimator *")]
        public static extern LDKFeeEstimator* ChannelManagerReadArgs_get_fee_estimator([NativeTypeName("const LDKChannelManagerReadArgs *")] LDKChannelManagerReadArgs* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManagerReadArgs_set_fee_estimator([NativeTypeName("LDKChannelManagerReadArgs *")] LDKChannelManagerReadArgs* this_ptr, LDKFeeEstimator val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const LDKManyChannelMonitor *")]
        public static extern LDKManyChannelMonitor* ChannelManagerReadArgs_get_monitor([NativeTypeName("const LDKChannelManagerReadArgs *")] LDKChannelManagerReadArgs* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManagerReadArgs_set_monitor([NativeTypeName("LDKChannelManagerReadArgs *")] LDKChannelManagerReadArgs* this_ptr, LDKManyChannelMonitor val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const LDKBroadcasterInterface *")]
        public static extern LDKBroadcasterInterface* ChannelManagerReadArgs_get_tx_broadcaster([NativeTypeName("const LDKChannelManagerReadArgs *")] LDKChannelManagerReadArgs* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManagerReadArgs_set_tx_broadcaster([NativeTypeName("LDKChannelManagerReadArgs *")] LDKChannelManagerReadArgs* this_ptr, LDKBroadcasterInterface val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const LDKLogger *")]
        public static extern LDKLogger* ChannelManagerReadArgs_get_logger([NativeTypeName("const LDKChannelManagerReadArgs *")] LDKChannelManagerReadArgs* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManagerReadArgs_set_logger([NativeTypeName("LDKChannelManagerReadArgs *")] LDKChannelManagerReadArgs* this_ptr, LDKLogger val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUserConfig ChannelManagerReadArgs_get_default_config([NativeTypeName("const LDKChannelManagerReadArgs *")] LDKChannelManagerReadArgs* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelManagerReadArgs_set_default_config([NativeTypeName("LDKChannelManagerReadArgs *")] LDKChannelManagerReadArgs* this_ptr, LDKUserConfig val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelManagerReadArgs ChannelManagerReadArgs_new(LDKKeysInterface keys_manager, LDKFeeEstimator fee_estimator, LDKManyChannelMonitor monitor, LDKBroadcasterInterface tx_broadcaster, LDKLogger logger, LDKUserConfig default_config, [NativeTypeName("LDKCVec_ChannelMonitorZ")] LDKCVecTempl_ChannelMonitor channel_monitors);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelMonitorUpdate_free(LDKChannelMonitorUpdate this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelMonitorUpdate_get_update_id([NativeTypeName("const LDKChannelMonitorUpdate *")] LDKChannelMonitorUpdate* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelMonitorUpdate_set_update_id([NativeTypeName("LDKChannelMonitorUpdate *")] LDKChannelMonitorUpdate* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 ChannelMonitorUpdate_write([NativeTypeName("const LDKChannelMonitorUpdate *")] LDKChannelMonitorUpdate* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelMonitorUpdate ChannelMonitorUpdate_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MonitorUpdateError_free(LDKMonitorUpdateError this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MonitorEvent_free(LDKMonitorEvent this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void HTLCUpdate_free(LDKHTLCUpdate this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 HTLCUpdate_write([NativeTypeName("const LDKHTLCUpdate *")] LDKHTLCUpdate* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKHTLCUpdate HTLCUpdate_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelMonitor_free(LDKChannelMonitor this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ManyChannelMonitor_free(LDKManyChannelMonitor this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_NoneMonitorUpdateErrorZ")]
        public static extern LDKCResultTempl_u8__MonitorUpdateError ChannelMonitor_update_monitor([NativeTypeName("LDKChannelMonitor *")] LDKChannelMonitor* this_arg, LDKChannelMonitorUpdate updates, [NativeTypeName("const LDKBroadcasterInterface *")] LDKBroadcasterInterface* broadcaster, [NativeTypeName("const LDKLogger *")] LDKLogger* logger);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelMonitor_get_latest_update_id([NativeTypeName("const LDKChannelMonitor *")] LDKChannelMonitor* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKC2Tuple_OutPointScriptZ")]
        public static extern LDKC2TupleTempl_OutPoint__CVec_u8Z ChannelMonitor_get_funding_txo([NativeTypeName("const LDKChannelMonitor *")] LDKChannelMonitor* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_MonitorEventZ")]
        public static extern LDKCVecTempl_MonitorEvent ChannelMonitor_get_and_clear_pending_monitor_events([NativeTypeName("LDKChannelMonitor *")] LDKChannelMonitor* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_EventZ")]
        public static extern LDKCVecTempl_Event ChannelMonitor_get_and_clear_pending_events([NativeTypeName("LDKChannelMonitor *")] LDKChannelMonitor* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_TransactionZ")]
        public static extern LDKCVecTempl_CVec_u8Z ChannelMonitor_get_latest_local_commitment_txn([NativeTypeName("LDKChannelMonitor *")] LDKChannelMonitor* this_arg, [NativeTypeName("const LDKLogger *")] LDKLogger* logger);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void DecodeError_free(LDKDecodeError this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Init_free(LDKInit this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ErrorMessage_free(LDKErrorMessage this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** ErrorMessage_get_channel_id([NativeTypeName("const LDKErrorMessage *")] LDKErrorMessage* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ErrorMessage_set_channel_id([NativeTypeName("LDKErrorMessage *")] LDKErrorMessage* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKStr ErrorMessage_get_data([NativeTypeName("const LDKErrorMessage *")] LDKErrorMessage* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ErrorMessage_set_data([NativeTypeName("LDKErrorMessage *")] LDKErrorMessage* this_ptr, [NativeTypeName("LDKCVec_u8Z")] LDKCVecTempl_u8 val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKErrorMessage ErrorMessage_new(LDKThirtyTwoBytes channel_id_arg, [NativeTypeName("LDKCVec_u8Z")] LDKCVecTempl_u8 data_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Ping_free(LDKPing this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort Ping_get_ponglen([NativeTypeName("const LDKPing *")] LDKPing* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Ping_set_ponglen([NativeTypeName("LDKPing *")] LDKPing* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort Ping_get_byteslen([NativeTypeName("const LDKPing *")] LDKPing* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Ping_set_byteslen([NativeTypeName("LDKPing *")] LDKPing* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPing Ping_new([NativeTypeName("uint16_t")] ushort ponglen_arg, [NativeTypeName("uint16_t")] ushort byteslen_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Pong_free(LDKPong this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort Pong_get_byteslen([NativeTypeName("const LDKPong *")] LDKPong* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Pong_set_byteslen([NativeTypeName("LDKPong *")] LDKPong* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPong Pong_new([NativeTypeName("uint16_t")] ushort byteslen_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_free(LDKOpenChannel this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** OpenChannel_get_chain_hash([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_chain_hash([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** OpenChannel_get_temporary_channel_id([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_temporary_channel_id([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong OpenChannel_get_funding_satoshis([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_funding_satoshis([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong OpenChannel_get_push_msat([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_push_msat([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong OpenChannel_get_dust_limit_satoshis([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_dust_limit_satoshis([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong OpenChannel_get_max_htlc_value_in_flight_msat([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_max_htlc_value_in_flight_msat([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong OpenChannel_get_channel_reserve_satoshis([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_channel_reserve_satoshis([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong OpenChannel_get_htlc_minimum_msat([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_htlc_minimum_msat([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint OpenChannel_get_feerate_per_kw([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_feerate_per_kw([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort OpenChannel_get_to_self_delay([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_to_self_delay([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort OpenChannel_get_max_accepted_htlcs([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_max_accepted_htlcs([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey OpenChannel_get_funding_pubkey([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_funding_pubkey([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey OpenChannel_get_revocation_basepoint([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_revocation_basepoint([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey OpenChannel_get_payment_point([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_payment_point([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey OpenChannel_get_delayed_payment_basepoint([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_delayed_payment_basepoint([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey OpenChannel_get_htlc_basepoint([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_htlc_basepoint([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey OpenChannel_get_first_per_commitment_point([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_first_per_commitment_point([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint8_t")]
        public static extern byte OpenChannel_get_channel_flags([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void OpenChannel_set_channel_flags([NativeTypeName("LDKOpenChannel *")] LDKOpenChannel* this_ptr, [NativeTypeName("uint8_t")] byte val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_free(LDKAcceptChannel this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** AcceptChannel_get_temporary_channel_id([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_temporary_channel_id([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong AcceptChannel_get_dust_limit_satoshis([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_dust_limit_satoshis([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong AcceptChannel_get_max_htlc_value_in_flight_msat([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_max_htlc_value_in_flight_msat([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong AcceptChannel_get_channel_reserve_satoshis([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_channel_reserve_satoshis([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong AcceptChannel_get_htlc_minimum_msat([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_htlc_minimum_msat([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint AcceptChannel_get_minimum_depth([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_minimum_depth([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort AcceptChannel_get_to_self_delay([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_to_self_delay([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort AcceptChannel_get_max_accepted_htlcs([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_max_accepted_htlcs([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey AcceptChannel_get_funding_pubkey([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_funding_pubkey([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey AcceptChannel_get_revocation_basepoint([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_revocation_basepoint([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey AcceptChannel_get_payment_point([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_payment_point([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey AcceptChannel_get_delayed_payment_basepoint([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_delayed_payment_basepoint([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey AcceptChannel_get_htlc_basepoint([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_htlc_basepoint([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey AcceptChannel_get_first_per_commitment_point([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AcceptChannel_set_first_per_commitment_point([NativeTypeName("LDKAcceptChannel *")] LDKAcceptChannel* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void FundingCreated_free(LDKFundingCreated this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** FundingCreated_get_temporary_channel_id([NativeTypeName("const LDKFundingCreated *")] LDKFundingCreated* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void FundingCreated_set_temporary_channel_id([NativeTypeName("LDKFundingCreated *")] LDKFundingCreated* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** FundingCreated_get_funding_txid([NativeTypeName("const LDKFundingCreated *")] LDKFundingCreated* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void FundingCreated_set_funding_txid([NativeTypeName("LDKFundingCreated *")] LDKFundingCreated* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort FundingCreated_get_funding_output_index([NativeTypeName("const LDKFundingCreated *")] LDKFundingCreated* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void FundingCreated_set_funding_output_index([NativeTypeName("LDKFundingCreated *")] LDKFundingCreated* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature FundingCreated_get_signature([NativeTypeName("const LDKFundingCreated *")] LDKFundingCreated* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void FundingCreated_set_signature([NativeTypeName("LDKFundingCreated *")] LDKFundingCreated* this_ptr, LDKSignature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKFundingCreated FundingCreated_new(LDKThirtyTwoBytes temporary_channel_id_arg, LDKThirtyTwoBytes funding_txid_arg, [NativeTypeName("uint16_t")] ushort funding_output_index_arg, LDKSignature signature_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void FundingSigned_free(LDKFundingSigned this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** FundingSigned_get_channel_id([NativeTypeName("const LDKFundingSigned *")] LDKFundingSigned* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void FundingSigned_set_channel_id([NativeTypeName("LDKFundingSigned *")] LDKFundingSigned* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature FundingSigned_get_signature([NativeTypeName("const LDKFundingSigned *")] LDKFundingSigned* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void FundingSigned_set_signature([NativeTypeName("LDKFundingSigned *")] LDKFundingSigned* this_ptr, LDKSignature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKFundingSigned FundingSigned_new(LDKThirtyTwoBytes channel_id_arg, LDKSignature signature_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void FundingLocked_free(LDKFundingLocked this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** FundingLocked_get_channel_id([NativeTypeName("const LDKFundingLocked *")] LDKFundingLocked* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void FundingLocked_set_channel_id([NativeTypeName("LDKFundingLocked *")] LDKFundingLocked* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey FundingLocked_get_next_per_commitment_point([NativeTypeName("const LDKFundingLocked *")] LDKFundingLocked* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void FundingLocked_set_next_per_commitment_point([NativeTypeName("LDKFundingLocked *")] LDKFundingLocked* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKFundingLocked FundingLocked_new(LDKThirtyTwoBytes channel_id_arg, LDKPublicKey next_per_commitment_point_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Shutdown_free(LDKShutdown this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** Shutdown_get_channel_id([NativeTypeName("const LDKShutdown *")] LDKShutdown* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Shutdown_set_channel_id([NativeTypeName("LDKShutdown *")] LDKShutdown* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKu8slice Shutdown_get_scriptpubkey([NativeTypeName("const LDKShutdown *")] LDKShutdown* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Shutdown_set_scriptpubkey([NativeTypeName("LDKShutdown *")] LDKShutdown* this_ptr, [NativeTypeName("LDKCVec_u8Z")] LDKCVecTempl_u8 val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKShutdown Shutdown_new(LDKThirtyTwoBytes channel_id_arg, [NativeTypeName("LDKCVec_u8Z")] LDKCVecTempl_u8 scriptpubkey_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ClosingSigned_free(LDKClosingSigned this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** ClosingSigned_get_channel_id([NativeTypeName("const LDKClosingSigned *")] LDKClosingSigned* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ClosingSigned_set_channel_id([NativeTypeName("LDKClosingSigned *")] LDKClosingSigned* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ClosingSigned_get_fee_satoshis([NativeTypeName("const LDKClosingSigned *")] LDKClosingSigned* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ClosingSigned_set_fee_satoshis([NativeTypeName("LDKClosingSigned *")] LDKClosingSigned* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature ClosingSigned_get_signature([NativeTypeName("const LDKClosingSigned *")] LDKClosingSigned* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ClosingSigned_set_signature([NativeTypeName("LDKClosingSigned *")] LDKClosingSigned* this_ptr, LDKSignature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKClosingSigned ClosingSigned_new(LDKThirtyTwoBytes channel_id_arg, [NativeTypeName("uint64_t")] ulong fee_satoshis_arg, LDKSignature signature_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateAddHTLC_free(LDKUpdateAddHTLC this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** UpdateAddHTLC_get_channel_id([NativeTypeName("const LDKUpdateAddHTLC *")] LDKUpdateAddHTLC* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateAddHTLC_set_channel_id([NativeTypeName("LDKUpdateAddHTLC *")] LDKUpdateAddHTLC* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong UpdateAddHTLC_get_htlc_id([NativeTypeName("const LDKUpdateAddHTLC *")] LDKUpdateAddHTLC* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateAddHTLC_set_htlc_id([NativeTypeName("LDKUpdateAddHTLC *")] LDKUpdateAddHTLC* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong UpdateAddHTLC_get_amount_msat([NativeTypeName("const LDKUpdateAddHTLC *")] LDKUpdateAddHTLC* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateAddHTLC_set_amount_msat([NativeTypeName("LDKUpdateAddHTLC *")] LDKUpdateAddHTLC* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** UpdateAddHTLC_get_payment_hash([NativeTypeName("const LDKUpdateAddHTLC *")] LDKUpdateAddHTLC* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateAddHTLC_set_payment_hash([NativeTypeName("LDKUpdateAddHTLC *")] LDKUpdateAddHTLC* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint UpdateAddHTLC_get_cltv_expiry([NativeTypeName("const LDKUpdateAddHTLC *")] LDKUpdateAddHTLC* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateAddHTLC_set_cltv_expiry([NativeTypeName("LDKUpdateAddHTLC *")] LDKUpdateAddHTLC* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFulfillHTLC_free(LDKUpdateFulfillHTLC this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** UpdateFulfillHTLC_get_channel_id([NativeTypeName("const LDKUpdateFulfillHTLC *")] LDKUpdateFulfillHTLC* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFulfillHTLC_set_channel_id([NativeTypeName("LDKUpdateFulfillHTLC *")] LDKUpdateFulfillHTLC* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong UpdateFulfillHTLC_get_htlc_id([NativeTypeName("const LDKUpdateFulfillHTLC *")] LDKUpdateFulfillHTLC* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFulfillHTLC_set_htlc_id([NativeTypeName("LDKUpdateFulfillHTLC *")] LDKUpdateFulfillHTLC* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** UpdateFulfillHTLC_get_payment_preimage([NativeTypeName("const LDKUpdateFulfillHTLC *")] LDKUpdateFulfillHTLC* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFulfillHTLC_set_payment_preimage([NativeTypeName("LDKUpdateFulfillHTLC *")] LDKUpdateFulfillHTLC* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUpdateFulfillHTLC UpdateFulfillHTLC_new(LDKThirtyTwoBytes channel_id_arg, [NativeTypeName("uint64_t")] ulong htlc_id_arg, LDKThirtyTwoBytes payment_preimage_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFailHTLC_free(LDKUpdateFailHTLC this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** UpdateFailHTLC_get_channel_id([NativeTypeName("const LDKUpdateFailHTLC *")] LDKUpdateFailHTLC* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFailHTLC_set_channel_id([NativeTypeName("LDKUpdateFailHTLC *")] LDKUpdateFailHTLC* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong UpdateFailHTLC_get_htlc_id([NativeTypeName("const LDKUpdateFailHTLC *")] LDKUpdateFailHTLC* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFailHTLC_set_htlc_id([NativeTypeName("LDKUpdateFailHTLC *")] LDKUpdateFailHTLC* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFailMalformedHTLC_free(LDKUpdateFailMalformedHTLC this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** UpdateFailMalformedHTLC_get_channel_id([NativeTypeName("const LDKUpdateFailMalformedHTLC *")] LDKUpdateFailMalformedHTLC* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFailMalformedHTLC_set_channel_id([NativeTypeName("LDKUpdateFailMalformedHTLC *")] LDKUpdateFailMalformedHTLC* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong UpdateFailMalformedHTLC_get_htlc_id([NativeTypeName("const LDKUpdateFailMalformedHTLC *")] LDKUpdateFailMalformedHTLC* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFailMalformedHTLC_set_htlc_id([NativeTypeName("LDKUpdateFailMalformedHTLC *")] LDKUpdateFailMalformedHTLC* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort UpdateFailMalformedHTLC_get_failure_code([NativeTypeName("const LDKUpdateFailMalformedHTLC *")] LDKUpdateFailMalformedHTLC* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFailMalformedHTLC_set_failure_code([NativeTypeName("LDKUpdateFailMalformedHTLC *")] LDKUpdateFailMalformedHTLC* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void CommitmentSigned_free(LDKCommitmentSigned this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** CommitmentSigned_get_channel_id([NativeTypeName("const LDKCommitmentSigned *")] LDKCommitmentSigned* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void CommitmentSigned_set_channel_id([NativeTypeName("LDKCommitmentSigned *")] LDKCommitmentSigned* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature CommitmentSigned_get_signature([NativeTypeName("const LDKCommitmentSigned *")] LDKCommitmentSigned* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void CommitmentSigned_set_signature([NativeTypeName("LDKCommitmentSigned *")] LDKCommitmentSigned* this_ptr, LDKSignature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void CommitmentSigned_set_htlc_signatures([NativeTypeName("LDKCommitmentSigned *")] LDKCommitmentSigned* this_ptr, [NativeTypeName("LDKCVec_SignatureZ")] LDKCVecTempl_Signature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKCommitmentSigned CommitmentSigned_new(LDKThirtyTwoBytes channel_id_arg, LDKSignature signature_arg, [NativeTypeName("LDKCVec_SignatureZ")] LDKCVecTempl_Signature htlc_signatures_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RevokeAndACK_free(LDKRevokeAndACK this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** RevokeAndACK_get_channel_id([NativeTypeName("const LDKRevokeAndACK *")] LDKRevokeAndACK* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RevokeAndACK_set_channel_id([NativeTypeName("LDKRevokeAndACK *")] LDKRevokeAndACK* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** RevokeAndACK_get_per_commitment_secret([NativeTypeName("const LDKRevokeAndACK *")] LDKRevokeAndACK* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RevokeAndACK_set_per_commitment_secret([NativeTypeName("LDKRevokeAndACK *")] LDKRevokeAndACK* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey RevokeAndACK_get_next_per_commitment_point([NativeTypeName("const LDKRevokeAndACK *")] LDKRevokeAndACK* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RevokeAndACK_set_next_per_commitment_point([NativeTypeName("LDKRevokeAndACK *")] LDKRevokeAndACK* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKRevokeAndACK RevokeAndACK_new(LDKThirtyTwoBytes channel_id_arg, LDKThirtyTwoBytes per_commitment_secret_arg, LDKPublicKey next_per_commitment_point_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFee_free(LDKUpdateFee this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** UpdateFee_get_channel_id([NativeTypeName("const LDKUpdateFee *")] LDKUpdateFee* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFee_set_channel_id([NativeTypeName("LDKUpdateFee *")] LDKUpdateFee* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint UpdateFee_get_feerate_per_kw([NativeTypeName("const LDKUpdateFee *")] LDKUpdateFee* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UpdateFee_set_feerate_per_kw([NativeTypeName("LDKUpdateFee *")] LDKUpdateFee* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUpdateFee UpdateFee_new(LDKThirtyTwoBytes channel_id_arg, [NativeTypeName("uint32_t")] uint feerate_per_kw_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void DataLossProtect_free(LDKDataLossProtect this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** DataLossProtect_get_your_last_per_commitment_secret([NativeTypeName("const LDKDataLossProtect *")] LDKDataLossProtect* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void DataLossProtect_set_your_last_per_commitment_secret([NativeTypeName("LDKDataLossProtect *")] LDKDataLossProtect* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey DataLossProtect_get_my_current_per_commitment_point([NativeTypeName("const LDKDataLossProtect *")] LDKDataLossProtect* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void DataLossProtect_set_my_current_per_commitment_point([NativeTypeName("LDKDataLossProtect *")] LDKDataLossProtect* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKDataLossProtect DataLossProtect_new(LDKThirtyTwoBytes your_last_per_commitment_secret_arg, LDKPublicKey my_current_per_commitment_point_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelReestablish_free(LDKChannelReestablish this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** ChannelReestablish_get_channel_id([NativeTypeName("const LDKChannelReestablish *")] LDKChannelReestablish* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelReestablish_set_channel_id([NativeTypeName("LDKChannelReestablish *")] LDKChannelReestablish* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelReestablish_get_next_local_commitment_number([NativeTypeName("const LDKChannelReestablish *")] LDKChannelReestablish* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelReestablish_set_next_local_commitment_number([NativeTypeName("LDKChannelReestablish *")] LDKChannelReestablish* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong ChannelReestablish_get_next_remote_commitment_number([NativeTypeName("const LDKChannelReestablish *")] LDKChannelReestablish* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelReestablish_set_next_remote_commitment_number([NativeTypeName("LDKChannelReestablish *")] LDKChannelReestablish* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AnnouncementSignatures_free(LDKAnnouncementSignatures this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** AnnouncementSignatures_get_channel_id([NativeTypeName("const LDKAnnouncementSignatures *")] LDKAnnouncementSignatures* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AnnouncementSignatures_set_channel_id([NativeTypeName("LDKAnnouncementSignatures *")] LDKAnnouncementSignatures* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong AnnouncementSignatures_get_short_channel_id([NativeTypeName("const LDKAnnouncementSignatures *")] LDKAnnouncementSignatures* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AnnouncementSignatures_set_short_channel_id([NativeTypeName("LDKAnnouncementSignatures *")] LDKAnnouncementSignatures* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature AnnouncementSignatures_get_node_signature([NativeTypeName("const LDKAnnouncementSignatures *")] LDKAnnouncementSignatures* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AnnouncementSignatures_set_node_signature([NativeTypeName("LDKAnnouncementSignatures *")] LDKAnnouncementSignatures* this_ptr, LDKSignature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature AnnouncementSignatures_get_bitcoin_signature([NativeTypeName("const LDKAnnouncementSignatures *")] LDKAnnouncementSignatures* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void AnnouncementSignatures_set_bitcoin_signature([NativeTypeName("LDKAnnouncementSignatures *")] LDKAnnouncementSignatures* this_ptr, LDKSignature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKAnnouncementSignatures AnnouncementSignatures_new(LDKThirtyTwoBytes channel_id_arg, [NativeTypeName("uint64_t")] ulong short_channel_id_arg, LDKSignature node_signature_arg, LDKSignature bitcoin_signature_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NetAddress_free(LDKNetAddress this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedNodeAnnouncement_free(LDKUnsignedNodeAnnouncement this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint UnsignedNodeAnnouncement_get_timestamp([NativeTypeName("const LDKUnsignedNodeAnnouncement *")] LDKUnsignedNodeAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedNodeAnnouncement_set_timestamp([NativeTypeName("LDKUnsignedNodeAnnouncement *")] LDKUnsignedNodeAnnouncement* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey UnsignedNodeAnnouncement_get_node_id([NativeTypeName("const LDKUnsignedNodeAnnouncement *")] LDKUnsignedNodeAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedNodeAnnouncement_set_node_id([NativeTypeName("LDKUnsignedNodeAnnouncement *")] LDKUnsignedNodeAnnouncement* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[3]")]
        public static extern byte** UnsignedNodeAnnouncement_get_rgb([NativeTypeName("const LDKUnsignedNodeAnnouncement *")] LDKUnsignedNodeAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedNodeAnnouncement_set_rgb([NativeTypeName("LDKUnsignedNodeAnnouncement *")] LDKUnsignedNodeAnnouncement* this_ptr, LDKThreeBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** UnsignedNodeAnnouncement_get_alias([NativeTypeName("const LDKUnsignedNodeAnnouncement *")] LDKUnsignedNodeAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedNodeAnnouncement_set_alias([NativeTypeName("LDKUnsignedNodeAnnouncement *")] LDKUnsignedNodeAnnouncement* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedNodeAnnouncement_set_addresses([NativeTypeName("LDKUnsignedNodeAnnouncement *")] LDKUnsignedNodeAnnouncement* this_ptr, [NativeTypeName("LDKCVec_NetAddressZ")] LDKCVecTempl_NetAddress val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeAnnouncement_free(LDKNodeAnnouncement this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature NodeAnnouncement_get_signature([NativeTypeName("const LDKNodeAnnouncement *")] LDKNodeAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeAnnouncement_set_signature([NativeTypeName("LDKNodeAnnouncement *")] LDKNodeAnnouncement* this_ptr, LDKSignature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUnsignedNodeAnnouncement NodeAnnouncement_get_contents([NativeTypeName("const LDKNodeAnnouncement *")] LDKNodeAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeAnnouncement_set_contents([NativeTypeName("LDKNodeAnnouncement *")] LDKNodeAnnouncement* this_ptr, LDKUnsignedNodeAnnouncement val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKNodeAnnouncement NodeAnnouncement_new(LDKSignature signature_arg, LDKUnsignedNodeAnnouncement contents_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelAnnouncement_free(LDKUnsignedChannelAnnouncement this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** UnsignedChannelAnnouncement_get_chain_hash([NativeTypeName("const LDKUnsignedChannelAnnouncement *")] LDKUnsignedChannelAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelAnnouncement_set_chain_hash([NativeTypeName("LDKUnsignedChannelAnnouncement *")] LDKUnsignedChannelAnnouncement* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong UnsignedChannelAnnouncement_get_short_channel_id([NativeTypeName("const LDKUnsignedChannelAnnouncement *")] LDKUnsignedChannelAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelAnnouncement_set_short_channel_id([NativeTypeName("LDKUnsignedChannelAnnouncement *")] LDKUnsignedChannelAnnouncement* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey UnsignedChannelAnnouncement_get_node_id_1([NativeTypeName("const LDKUnsignedChannelAnnouncement *")] LDKUnsignedChannelAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelAnnouncement_set_node_id_1([NativeTypeName("LDKUnsignedChannelAnnouncement *")] LDKUnsignedChannelAnnouncement* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey UnsignedChannelAnnouncement_get_node_id_2([NativeTypeName("const LDKUnsignedChannelAnnouncement *")] LDKUnsignedChannelAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelAnnouncement_set_node_id_2([NativeTypeName("LDKUnsignedChannelAnnouncement *")] LDKUnsignedChannelAnnouncement* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey UnsignedChannelAnnouncement_get_bitcoin_key_1([NativeTypeName("const LDKUnsignedChannelAnnouncement *")] LDKUnsignedChannelAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelAnnouncement_set_bitcoin_key_1([NativeTypeName("LDKUnsignedChannelAnnouncement *")] LDKUnsignedChannelAnnouncement* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey UnsignedChannelAnnouncement_get_bitcoin_key_2([NativeTypeName("const LDKUnsignedChannelAnnouncement *")] LDKUnsignedChannelAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelAnnouncement_set_bitcoin_key_2([NativeTypeName("LDKUnsignedChannelAnnouncement *")] LDKUnsignedChannelAnnouncement* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelAnnouncement_free(LDKChannelAnnouncement this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature ChannelAnnouncement_get_node_signature_1([NativeTypeName("const LDKChannelAnnouncement *")] LDKChannelAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelAnnouncement_set_node_signature_1([NativeTypeName("LDKChannelAnnouncement *")] LDKChannelAnnouncement* this_ptr, LDKSignature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature ChannelAnnouncement_get_node_signature_2([NativeTypeName("const LDKChannelAnnouncement *")] LDKChannelAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelAnnouncement_set_node_signature_2([NativeTypeName("LDKChannelAnnouncement *")] LDKChannelAnnouncement* this_ptr, LDKSignature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature ChannelAnnouncement_get_bitcoin_signature_1([NativeTypeName("const LDKChannelAnnouncement *")] LDKChannelAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelAnnouncement_set_bitcoin_signature_1([NativeTypeName("LDKChannelAnnouncement *")] LDKChannelAnnouncement* this_ptr, LDKSignature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature ChannelAnnouncement_get_bitcoin_signature_2([NativeTypeName("const LDKChannelAnnouncement *")] LDKChannelAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelAnnouncement_set_bitcoin_signature_2([NativeTypeName("LDKChannelAnnouncement *")] LDKChannelAnnouncement* this_ptr, LDKSignature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUnsignedChannelAnnouncement ChannelAnnouncement_get_contents([NativeTypeName("const LDKChannelAnnouncement *")] LDKChannelAnnouncement* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelAnnouncement_set_contents([NativeTypeName("LDKChannelAnnouncement *")] LDKChannelAnnouncement* this_ptr, LDKUnsignedChannelAnnouncement val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelAnnouncement ChannelAnnouncement_new(LDKSignature node_signature_1_arg, LDKSignature node_signature_2_arg, LDKSignature bitcoin_signature_1_arg, LDKSignature bitcoin_signature_2_arg, LDKUnsignedChannelAnnouncement contents_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelUpdate_free(LDKUnsignedChannelUpdate this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** UnsignedChannelUpdate_get_chain_hash([NativeTypeName("const LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelUpdate_set_chain_hash([NativeTypeName("LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong UnsignedChannelUpdate_get_short_channel_id([NativeTypeName("const LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelUpdate_set_short_channel_id([NativeTypeName("LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint UnsignedChannelUpdate_get_timestamp([NativeTypeName("const LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelUpdate_set_timestamp([NativeTypeName("LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint8_t")]
        public static extern byte UnsignedChannelUpdate_get_flags([NativeTypeName("const LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelUpdate_set_flags([NativeTypeName("LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr, [NativeTypeName("uint8_t")] byte val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort UnsignedChannelUpdate_get_cltv_expiry_delta([NativeTypeName("const LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelUpdate_set_cltv_expiry_delta([NativeTypeName("LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong UnsignedChannelUpdate_get_htlc_minimum_msat([NativeTypeName("const LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelUpdate_set_htlc_minimum_msat([NativeTypeName("LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint UnsignedChannelUpdate_get_fee_base_msat([NativeTypeName("const LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelUpdate_set_fee_base_msat([NativeTypeName("LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint UnsignedChannelUpdate_get_fee_proportional_millionths([NativeTypeName("const LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void UnsignedChannelUpdate_set_fee_proportional_millionths([NativeTypeName("LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelUpdate_free(LDKChannelUpdate this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature ChannelUpdate_get_signature([NativeTypeName("const LDKChannelUpdate *")] LDKChannelUpdate* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelUpdate_set_signature([NativeTypeName("LDKChannelUpdate *")] LDKChannelUpdate* this_ptr, LDKSignature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUnsignedChannelUpdate ChannelUpdate_get_contents([NativeTypeName("const LDKChannelUpdate *")] LDKChannelUpdate* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelUpdate_set_contents([NativeTypeName("LDKChannelUpdate *")] LDKChannelUpdate* this_ptr, LDKUnsignedChannelUpdate val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelUpdate ChannelUpdate_new(LDKSignature signature_arg, LDKUnsignedChannelUpdate contents_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ErrorAction_free(LDKErrorAction this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void LightningError_free(LDKLightningError this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKStr LightningError_get_err([NativeTypeName("const LDKLightningError *")] LDKLightningError* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void LightningError_set_err([NativeTypeName("LDKLightningError *")] LDKLightningError* this_ptr, [NativeTypeName("LDKCVec_u8Z")] LDKCVecTempl_u8 val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKErrorAction LightningError_get_action([NativeTypeName("const LDKLightningError *")] LDKLightningError* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void LightningError_set_action([NativeTypeName("LDKLightningError *")] LDKLightningError* this_ptr, LDKErrorAction val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKLightningError LightningError_new([NativeTypeName("LDKCVec_u8Z")] LDKCVecTempl_u8 err_arg, LDKErrorAction action_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void CommitmentUpdate_free(LDKCommitmentUpdate this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void CommitmentUpdate_set_update_add_htlcs([NativeTypeName("LDKCommitmentUpdate *")] LDKCommitmentUpdate* this_ptr, [NativeTypeName("LDKCVec_UpdateAddHTLCZ")] LDKCVecTempl_UpdateAddHTLC val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void CommitmentUpdate_set_update_fulfill_htlcs([NativeTypeName("LDKCommitmentUpdate *")] LDKCommitmentUpdate* this_ptr, [NativeTypeName("LDKCVec_UpdateFulfillHTLCZ")] LDKCVecTempl_UpdateFulfillHTLC val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void CommitmentUpdate_set_update_fail_htlcs([NativeTypeName("LDKCommitmentUpdate *")] LDKCommitmentUpdate* this_ptr, [NativeTypeName("LDKCVec_UpdateFailHTLCZ")] LDKCVecTempl_UpdateFailHTLC val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void CommitmentUpdate_set_update_fail_malformed_htlcs([NativeTypeName("LDKCommitmentUpdate *")] LDKCommitmentUpdate* this_ptr, [NativeTypeName("LDKCVec_UpdateFailMalformedHTLCZ")] LDKCVecTempl_UpdateFailMalformedHTLC val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUpdateFee CommitmentUpdate_get_update_fee([NativeTypeName("const LDKCommitmentUpdate *")] LDKCommitmentUpdate* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void CommitmentUpdate_set_update_fee([NativeTypeName("LDKCommitmentUpdate *")] LDKCommitmentUpdate* this_ptr, LDKUpdateFee val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKCommitmentSigned CommitmentUpdate_get_commitment_signed([NativeTypeName("const LDKCommitmentUpdate *")] LDKCommitmentUpdate* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void CommitmentUpdate_set_commitment_signed([NativeTypeName("LDKCommitmentUpdate *")] LDKCommitmentUpdate* this_ptr, LDKCommitmentSigned val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKCommitmentUpdate CommitmentUpdate_new([NativeTypeName("LDKCVec_UpdateAddHTLCZ")] LDKCVecTempl_UpdateAddHTLC update_add_htlcs_arg, [NativeTypeName("LDKCVec_UpdateFulfillHTLCZ")] LDKCVecTempl_UpdateFulfillHTLC update_fulfill_htlcs_arg, [NativeTypeName("LDKCVec_UpdateFailHTLCZ")] LDKCVecTempl_UpdateFailHTLC update_fail_htlcs_arg, [NativeTypeName("LDKCVec_UpdateFailMalformedHTLCZ")] LDKCVecTempl_UpdateFailMalformedHTLC update_fail_malformed_htlcs_arg, LDKUpdateFee update_fee_arg, LDKCommitmentSigned commitment_signed_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void HTLCFailChannelUpdate_free(LDKHTLCFailChannelUpdate this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelMessageHandler_free(LDKChannelMessageHandler this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RoutingMessageHandler_free(LDKRoutingMessageHandler this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 AcceptChannel_write([NativeTypeName("const LDKAcceptChannel *")] LDKAcceptChannel* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKAcceptChannel AcceptChannel_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 AnnouncementSignatures_write([NativeTypeName("const LDKAnnouncementSignatures *")] LDKAnnouncementSignatures* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKAnnouncementSignatures AnnouncementSignatures_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 ChannelReestablish_write([NativeTypeName("const LDKChannelReestablish *")] LDKChannelReestablish* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelReestablish ChannelReestablish_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 ClosingSigned_write([NativeTypeName("const LDKClosingSigned *")] LDKClosingSigned* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKClosingSigned ClosingSigned_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 CommitmentSigned_write([NativeTypeName("const LDKCommitmentSigned *")] LDKCommitmentSigned* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKCommitmentSigned CommitmentSigned_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 FundingCreated_write([NativeTypeName("const LDKFundingCreated *")] LDKFundingCreated* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKFundingCreated FundingCreated_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 FundingSigned_write([NativeTypeName("const LDKFundingSigned *")] LDKFundingSigned* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKFundingSigned FundingSigned_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 FundingLocked_write([NativeTypeName("const LDKFundingLocked *")] LDKFundingLocked* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKFundingLocked FundingLocked_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 Init_write([NativeTypeName("const LDKInit *")] LDKInit* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKInit Init_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 OpenChannel_write([NativeTypeName("const LDKOpenChannel *")] LDKOpenChannel* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKOpenChannel OpenChannel_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 RevokeAndACK_write([NativeTypeName("const LDKRevokeAndACK *")] LDKRevokeAndACK* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKRevokeAndACK RevokeAndACK_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 Shutdown_write([NativeTypeName("const LDKShutdown *")] LDKShutdown* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKShutdown Shutdown_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 UpdateFailHTLC_write([NativeTypeName("const LDKUpdateFailHTLC *")] LDKUpdateFailHTLC* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUpdateFailHTLC UpdateFailHTLC_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 UpdateFailMalformedHTLC_write([NativeTypeName("const LDKUpdateFailMalformedHTLC *")] LDKUpdateFailMalformedHTLC* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUpdateFailMalformedHTLC UpdateFailMalformedHTLC_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 UpdateFee_write([NativeTypeName("const LDKUpdateFee *")] LDKUpdateFee* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUpdateFee UpdateFee_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 UpdateFulfillHTLC_write([NativeTypeName("const LDKUpdateFulfillHTLC *")] LDKUpdateFulfillHTLC* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUpdateFulfillHTLC UpdateFulfillHTLC_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 UpdateAddHTLC_write([NativeTypeName("const LDKUpdateAddHTLC *")] LDKUpdateAddHTLC* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUpdateAddHTLC UpdateAddHTLC_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 Ping_write([NativeTypeName("const LDKPing *")] LDKPing* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPing Ping_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 Pong_write([NativeTypeName("const LDKPong *")] LDKPong* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPong Pong_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 UnsignedChannelAnnouncement_write([NativeTypeName("const LDKUnsignedChannelAnnouncement *")] LDKUnsignedChannelAnnouncement* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUnsignedChannelAnnouncement UnsignedChannelAnnouncement_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 ChannelAnnouncement_write([NativeTypeName("const LDKChannelAnnouncement *")] LDKChannelAnnouncement* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelAnnouncement ChannelAnnouncement_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 UnsignedChannelUpdate_write([NativeTypeName("const LDKUnsignedChannelUpdate *")] LDKUnsignedChannelUpdate* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUnsignedChannelUpdate UnsignedChannelUpdate_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 ChannelUpdate_write([NativeTypeName("const LDKChannelUpdate *")] LDKChannelUpdate* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelUpdate ChannelUpdate_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 ErrorMessage_write([NativeTypeName("const LDKErrorMessage *")] LDKErrorMessage* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKErrorMessage ErrorMessage_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 UnsignedNodeAnnouncement_write([NativeTypeName("const LDKUnsignedNodeAnnouncement *")] LDKUnsignedNodeAnnouncement* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKUnsignedNodeAnnouncement UnsignedNodeAnnouncement_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 NodeAnnouncement_write([NativeTypeName("const LDKNodeAnnouncement *")] LDKNodeAnnouncement* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKNodeAnnouncement NodeAnnouncement_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MessageHandler_free(LDKMessageHandler this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const LDKChannelMessageHandler *")]
        public static extern LDKChannelMessageHandler* MessageHandler_get_chan_handler([NativeTypeName("const LDKMessageHandler *")] LDKMessageHandler* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MessageHandler_set_chan_handler([NativeTypeName("LDKMessageHandler *")] LDKMessageHandler* this_ptr, LDKChannelMessageHandler val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const LDKRoutingMessageHandler *")]
        public static extern LDKRoutingMessageHandler* MessageHandler_get_route_handler([NativeTypeName("const LDKMessageHandler *")] LDKMessageHandler* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MessageHandler_set_route_handler([NativeTypeName("LDKMessageHandler *")] LDKMessageHandler* this_ptr, LDKRoutingMessageHandler val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKMessageHandler MessageHandler_new(LDKChannelMessageHandler chan_handler_arg, LDKRoutingMessageHandler route_handler_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void SocketDescriptor_free(LDKSocketDescriptor this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void PeerHandleError_free(LDKPeerHandleError this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte PeerHandleError_get_no_connection_possible([NativeTypeName("const LDKPeerHandleError *")] LDKPeerHandleError* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void PeerHandleError_set_no_connection_possible([NativeTypeName("LDKPeerHandleError *")] LDKPeerHandleError* this_ptr, [NativeTypeName("bool")] byte val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPeerHandleError PeerHandleError_new([NativeTypeName("bool")] byte no_connection_possible_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void PeerManager_free(LDKPeerManager this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPeerManager PeerManager_new(LDKMessageHandler message_handler, LDKSecretKey our_node_secret, [NativeTypeName("const uint8_t (*)[32]")] byte** ephemeral_random_data, LDKLogger logger);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_PublicKeyZ")]
        public static extern LDKCVecTempl_PublicKey PeerManager_get_peer_node_ids([NativeTypeName("const LDKPeerManager *")] LDKPeerManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_CVec_u8ZPeerHandleErrorZ")]
        public static extern LDKCResultTempl_CVecTempl_u8_____PeerHandleError PeerManager_new_outbound_connection([NativeTypeName("const LDKPeerManager *")] LDKPeerManager* this_arg, LDKPublicKey their_node_id, LDKSocketDescriptor descriptor);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_NonePeerHandleErrorZ")]
        public static extern LDKCResultTempl_u8__PeerHandleError PeerManager_new_inbound_connection([NativeTypeName("const LDKPeerManager *")] LDKPeerManager* this_arg, LDKSocketDescriptor descriptor);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_NonePeerHandleErrorZ")]
        public static extern LDKCResultTempl_u8__PeerHandleError PeerManager_write_buffer_space_avail([NativeTypeName("const LDKPeerManager *")] LDKPeerManager* this_arg, [NativeTypeName("LDKSocketDescriptor *")] LDKSocketDescriptor* descriptor);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_boolPeerHandleErrorZ")]
        public static extern LDKCResultTempl_bool__PeerHandleError PeerManager_read_event([NativeTypeName("const LDKPeerManager *")] LDKPeerManager* this_arg, [NativeTypeName("LDKSocketDescriptor *")] LDKSocketDescriptor* peer_descriptor, LDKu8slice data);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void PeerManager_process_events([NativeTypeName("const LDKPeerManager *")] LDKPeerManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void PeerManager_socket_disconnected([NativeTypeName("const LDKPeerManager *")] LDKPeerManager* this_arg, [NativeTypeName("const LDKSocketDescriptor *")] LDKSocketDescriptor* descriptor);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void PeerManager_timer_tick_occured([NativeTypeName("const LDKPeerManager *")] LDKPeerManager* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKThirtyTwoBytes build_commitment_secret([NativeTypeName("const uint8_t (*)[32]")] byte** commitment_seed, [NativeTypeName("uint64_t")] ulong idx);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_SecretKeySecpErrorZ")]
        public static extern LDKCResultTempl_SecretKey__Secp256k1Error derive_private_key(LDKPublicKey per_commitment_point, [NativeTypeName("const uint8_t (*)[32]")] byte** base_secret);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_PublicKeySecpErrorZ")]
        public static extern LDKCResultTempl_PublicKey__Secp256k1Error derive_public_key(LDKPublicKey per_commitment_point, LDKPublicKey base_point);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_SecretKeySecpErrorZ")]
        public static extern LDKCResultTempl_SecretKey__Secp256k1Error derive_private_revocation_key([NativeTypeName("const uint8_t (*)[32]")] byte** per_commitment_secret, [NativeTypeName("const uint8_t (*)[32]")] byte** revocation_base_secret);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_PublicKeySecpErrorZ")]
        public static extern LDKCResultTempl_PublicKey__Secp256k1Error derive_public_revocation_key(LDKPublicKey per_commitment_point, LDKPublicKey revocation_base_point);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void TxCreationKeys_free(LDKTxCreationKeys this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey TxCreationKeys_get_per_commitment_point([NativeTypeName("const LDKTxCreationKeys *")] LDKTxCreationKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void TxCreationKeys_set_per_commitment_point([NativeTypeName("LDKTxCreationKeys *")] LDKTxCreationKeys* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey TxCreationKeys_get_revocation_key([NativeTypeName("const LDKTxCreationKeys *")] LDKTxCreationKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void TxCreationKeys_set_revocation_key([NativeTypeName("LDKTxCreationKeys *")] LDKTxCreationKeys* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey TxCreationKeys_get_a_htlc_key([NativeTypeName("const LDKTxCreationKeys *")] LDKTxCreationKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void TxCreationKeys_set_a_htlc_key([NativeTypeName("LDKTxCreationKeys *")] LDKTxCreationKeys* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey TxCreationKeys_get_b_htlc_key([NativeTypeName("const LDKTxCreationKeys *")] LDKTxCreationKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void TxCreationKeys_set_b_htlc_key([NativeTypeName("LDKTxCreationKeys *")] LDKTxCreationKeys* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey TxCreationKeys_get_a_delayed_payment_key([NativeTypeName("const LDKTxCreationKeys *")] LDKTxCreationKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void TxCreationKeys_set_a_delayed_payment_key([NativeTypeName("LDKTxCreationKeys *")] LDKTxCreationKeys* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKTxCreationKeys TxCreationKeys_new(LDKPublicKey per_commitment_point_arg, LDKPublicKey revocation_key_arg, LDKPublicKey a_htlc_key_arg, LDKPublicKey b_htlc_key_arg, LDKPublicKey a_delayed_payment_key_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 TxCreationKeys_write([NativeTypeName("const LDKTxCreationKeys *")] LDKTxCreationKeys* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKTxCreationKeys TxCreationKeys_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void PreCalculatedTxCreationKeys_free(LDKPreCalculatedTxCreationKeys this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPreCalculatedTxCreationKeys PreCalculatedTxCreationKeys_new(LDKTxCreationKeys keys);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKTxCreationKeys PreCalculatedTxCreationKeys_trust_key_derivation([NativeTypeName("const LDKPreCalculatedTxCreationKeys *")] LDKPreCalculatedTxCreationKeys* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey PreCalculatedTxCreationKeys_per_commitment_point([NativeTypeName("const LDKPreCalculatedTxCreationKeys *")] LDKPreCalculatedTxCreationKeys* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelPublicKeys_free(LDKChannelPublicKeys this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey ChannelPublicKeys_get_funding_pubkey([NativeTypeName("const LDKChannelPublicKeys *")] LDKChannelPublicKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelPublicKeys_set_funding_pubkey([NativeTypeName("LDKChannelPublicKeys *")] LDKChannelPublicKeys* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey ChannelPublicKeys_get_revocation_basepoint([NativeTypeName("const LDKChannelPublicKeys *")] LDKChannelPublicKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelPublicKeys_set_revocation_basepoint([NativeTypeName("LDKChannelPublicKeys *")] LDKChannelPublicKeys* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey ChannelPublicKeys_get_payment_point([NativeTypeName("const LDKChannelPublicKeys *")] LDKChannelPublicKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelPublicKeys_set_payment_point([NativeTypeName("LDKChannelPublicKeys *")] LDKChannelPublicKeys* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey ChannelPublicKeys_get_delayed_payment_basepoint([NativeTypeName("const LDKChannelPublicKeys *")] LDKChannelPublicKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelPublicKeys_set_delayed_payment_basepoint([NativeTypeName("LDKChannelPublicKeys *")] LDKChannelPublicKeys* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey ChannelPublicKeys_get_htlc_basepoint([NativeTypeName("const LDKChannelPublicKeys *")] LDKChannelPublicKeys* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelPublicKeys_set_htlc_basepoint([NativeTypeName("LDKChannelPublicKeys *")] LDKChannelPublicKeys* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelPublicKeys ChannelPublicKeys_new(LDKPublicKey funding_pubkey_arg, LDKPublicKey revocation_basepoint_arg, LDKPublicKey payment_point_arg, LDKPublicKey delayed_payment_basepoint_arg, LDKPublicKey htlc_basepoint_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 ChannelPublicKeys_write([NativeTypeName("const LDKChannelPublicKeys *")] LDKChannelPublicKeys* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelPublicKeys ChannelPublicKeys_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_TxCreationKeysSecpErrorZ")]
        public static extern LDKCResultTempl_TxCreationKeys__Secp256k1Error TxCreationKeys_derive_new(LDKPublicKey per_commitment_point, LDKPublicKey a_delayed_payment_base, LDKPublicKey a_htlc_base, LDKPublicKey b_revocation_base, LDKPublicKey b_htlc_base);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 get_revokeable_redeemscript(LDKPublicKey revocation_key, [NativeTypeName("uint16_t")] ushort to_self_delay, LDKPublicKey delayed_payment_key);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void HTLCOutputInCommitment_free(LDKHTLCOutputInCommitment this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte HTLCOutputInCommitment_get_offered([NativeTypeName("const LDKHTLCOutputInCommitment *")] LDKHTLCOutputInCommitment* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void HTLCOutputInCommitment_set_offered([NativeTypeName("LDKHTLCOutputInCommitment *")] LDKHTLCOutputInCommitment* this_ptr, [NativeTypeName("bool")] byte val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong HTLCOutputInCommitment_get_amount_msat([NativeTypeName("const LDKHTLCOutputInCommitment *")] LDKHTLCOutputInCommitment* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void HTLCOutputInCommitment_set_amount_msat([NativeTypeName("LDKHTLCOutputInCommitment *")] LDKHTLCOutputInCommitment* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint HTLCOutputInCommitment_get_cltv_expiry([NativeTypeName("const LDKHTLCOutputInCommitment *")] LDKHTLCOutputInCommitment* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void HTLCOutputInCommitment_set_cltv_expiry([NativeTypeName("LDKHTLCOutputInCommitment *")] LDKHTLCOutputInCommitment* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** HTLCOutputInCommitment_get_payment_hash([NativeTypeName("const LDKHTLCOutputInCommitment *")] LDKHTLCOutputInCommitment* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void HTLCOutputInCommitment_set_payment_hash([NativeTypeName("LDKHTLCOutputInCommitment *")] LDKHTLCOutputInCommitment* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 HTLCOutputInCommitment_write([NativeTypeName("const LDKHTLCOutputInCommitment *")] LDKHTLCOutputInCommitment* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKHTLCOutputInCommitment HTLCOutputInCommitment_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 get_htlc_redeemscript([NativeTypeName("const LDKHTLCOutputInCommitment *")] LDKHTLCOutputInCommitment* htlc, [NativeTypeName("const LDKTxCreationKeys *")] LDKTxCreationKeys* keys);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 make_funding_redeemscript(LDKPublicKey a, LDKPublicKey b);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 build_htlc_transaction([NativeTypeName("const uint8_t (*)[32]")] byte** prev_hash, [NativeTypeName("uint32_t")] uint feerate_per_kw, [NativeTypeName("uint16_t")] ushort to_self_delay, [NativeTypeName("const LDKHTLCOutputInCommitment *")] LDKHTLCOutputInCommitment* htlc, LDKPublicKey a_delayed_payment_key, LDKPublicKey revocation_key);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void LocalCommitmentTransaction_free(LDKLocalCommitmentTransaction this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 LocalCommitmentTransaction_get_unsigned_tx([NativeTypeName("const LDKLocalCommitmentTransaction *")] LDKLocalCommitmentTransaction* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void LocalCommitmentTransaction_set_unsigned_tx([NativeTypeName("LDKLocalCommitmentTransaction *")] LDKLocalCommitmentTransaction* this_ptr, [NativeTypeName("LDKCVec_u8Z")] LDKCVecTempl_u8 val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature LocalCommitmentTransaction_get_their_sig([NativeTypeName("const LDKLocalCommitmentTransaction *")] LDKLocalCommitmentTransaction* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void LocalCommitmentTransaction_set_their_sig([NativeTypeName("LDKLocalCommitmentTransaction *")] LDKLocalCommitmentTransaction* this_ptr, LDKSignature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint LocalCommitmentTransaction_get_feerate_per_kw([NativeTypeName("const LDKLocalCommitmentTransaction *")] LDKLocalCommitmentTransaction* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void LocalCommitmentTransaction_set_feerate_per_kw([NativeTypeName("LDKLocalCommitmentTransaction *")] LDKLocalCommitmentTransaction* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void LocalCommitmentTransaction_set_per_htlc([NativeTypeName("LDKLocalCommitmentTransaction *")] LDKLocalCommitmentTransaction* this_ptr, [NativeTypeName("LDKCVec_C2Tuple_HTLCOutputInCommitmentSignatureZZ")] LDKCVecTempl_C2TupleTempl_HTLCOutputInCommitment__Signature val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKLocalCommitmentTransaction LocalCommitmentTransaction_new_missing_local_sig([NativeTypeName("LDKCVec_u8Z")] LDKCVecTempl_u8 unsigned_tx, LDKSignature their_sig, LDKPublicKey our_funding_key, LDKPublicKey their_funding_key, LDKTxCreationKeys local_keys, [NativeTypeName("uint32_t")] uint feerate_per_kw, [NativeTypeName("LDKCVec_C2Tuple_HTLCOutputInCommitmentSignatureZZ")] LDKCVecTempl_C2TupleTempl_HTLCOutputInCommitment__Signature htlc_data);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKTxCreationKeys LocalCommitmentTransaction_trust_key_derivation([NativeTypeName("const LDKLocalCommitmentTransaction *")] LDKLocalCommitmentTransaction* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKThirtyTwoBytes LocalCommitmentTransaction_txid([NativeTypeName("const LDKLocalCommitmentTransaction *")] LDKLocalCommitmentTransaction* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKSignature LocalCommitmentTransaction_get_local_sig([NativeTypeName("const LDKLocalCommitmentTransaction *")] LDKLocalCommitmentTransaction* this_arg, [NativeTypeName("const uint8_t (*)[32]")] byte** funding_key, LDKu8slice funding_redeemscript, [NativeTypeName("uint64_t")] ulong channel_value_satoshis);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_CVec_SignatureZNoneZ")]
        public static extern LDKCResultTempl_CVecTempl_Signature_____u8 LocalCommitmentTransaction_get_htlc_sigs([NativeTypeName("const LDKLocalCommitmentTransaction *")] LDKLocalCommitmentTransaction* this_arg, [NativeTypeName("const uint8_t (*)[32]")] byte** htlc_base_key, [NativeTypeName("uint16_t")] ushort local_csv);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 LocalCommitmentTransaction_write([NativeTypeName("const LDKLocalCommitmentTransaction *")] LDKLocalCommitmentTransaction* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKLocalCommitmentTransaction LocalCommitmentTransaction_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void InitFeatures_free(LDKInitFeatures this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeFeatures_free(LDKNodeFeatures this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelFeatures_free(LDKChannelFeatures this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RouteHop_free(LDKRouteHop this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey RouteHop_get_pubkey([NativeTypeName("const LDKRouteHop *")] LDKRouteHop* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RouteHop_set_pubkey([NativeTypeName("LDKRouteHop *")] LDKRouteHop* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong RouteHop_get_short_channel_id([NativeTypeName("const LDKRouteHop *")] LDKRouteHop* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RouteHop_set_short_channel_id([NativeTypeName("LDKRouteHop *")] LDKRouteHop* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong RouteHop_get_fee_msat([NativeTypeName("const LDKRouteHop *")] LDKRouteHop* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RouteHop_set_fee_msat([NativeTypeName("LDKRouteHop *")] LDKRouteHop* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint RouteHop_get_cltv_expiry_delta([NativeTypeName("const LDKRouteHop *")] LDKRouteHop* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RouteHop_set_cltv_expiry_delta([NativeTypeName("LDKRouteHop *")] LDKRouteHop* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Route_free(LDKRoute this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Route_set_paths([NativeTypeName("LDKRoute *")] LDKRoute* this_ptr, [NativeTypeName("LDKCVec_CVec_RouteHopZZ")] LDKCVecTempl_CVecTempl_RouteHop val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKRoute Route_new([NativeTypeName("LDKCVec_CVec_RouteHopZZ")] LDKCVecTempl_CVecTempl_RouteHop paths_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 Route_write([NativeTypeName("const LDKRoute *")] LDKRoute* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKRoute Route_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RouteHint_free(LDKRouteHint this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey RouteHint_get_src_node_id([NativeTypeName("const LDKRouteHint *")] LDKRouteHint* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RouteHint_set_src_node_id([NativeTypeName("LDKRouteHint *")] LDKRouteHint* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong RouteHint_get_short_channel_id([NativeTypeName("const LDKRouteHint *")] LDKRouteHint* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RouteHint_set_short_channel_id([NativeTypeName("LDKRouteHint *")] LDKRouteHint* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKRoutingFees RouteHint_get_fees([NativeTypeName("const LDKRouteHint *")] LDKRouteHint* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RouteHint_set_fees([NativeTypeName("LDKRouteHint *")] LDKRouteHint* this_ptr, LDKRoutingFees val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort RouteHint_get_cltv_expiry_delta([NativeTypeName("const LDKRouteHint *")] LDKRouteHint* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RouteHint_set_cltv_expiry_delta([NativeTypeName("LDKRouteHint *")] LDKRouteHint* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong RouteHint_get_htlc_minimum_msat([NativeTypeName("const LDKRouteHint *")] LDKRouteHint* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RouteHint_set_htlc_minimum_msat([NativeTypeName("LDKRouteHint *")] LDKRouteHint* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKRouteHint RouteHint_new(LDKPublicKey src_node_id_arg, [NativeTypeName("uint64_t")] ulong short_channel_id_arg, LDKRoutingFees fees_arg, [NativeTypeName("uint16_t")] ushort cltv_expiry_delta_arg, [NativeTypeName("uint64_t")] ulong htlc_minimum_msat_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCResult_RouteLightningErrorZ")]
        public static extern LDKCResultTempl_Route__LightningError get_route(LDKPublicKey our_node_id, [NativeTypeName("const LDKNetworkGraph *")] LDKNetworkGraph* network, LDKPublicKey target, [NativeTypeName("LDKCVec_ChannelDetailsZ *")] LDKCVecTempl_ChannelDetails* first_hops, [NativeTypeName("LDKCVec_RouteHintZ")] LDKCVecTempl_RouteHint last_hops, [NativeTypeName("uint64_t")] ulong final_value_msat, [NativeTypeName("uint32_t")] uint final_cltv, LDKLogger logger);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NetworkGraph_free(LDKNetworkGraph this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void LockedNetworkGraph_free(LDKLockedNetworkGraph this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NetGraphMsgHandler_free(LDKNetGraphMsgHandler this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKNetGraphMsgHandler NetGraphMsgHandler_new(LDKChainWatchInterface chain_monitor, LDKLogger logger);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKNetGraphMsgHandler NetGraphMsgHandler_from_net_graph(LDKChainWatchInterface chain_monitor, LDKLogger logger, LDKNetworkGraph network_graph);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKLockedNetworkGraph NetGraphMsgHandler_read_locked_graph([NativeTypeName("const LDKNetGraphMsgHandler *")] LDKNetGraphMsgHandler* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKNetworkGraph LockedNetworkGraph_graph([NativeTypeName("const LDKLockedNetworkGraph *")] LDKLockedNetworkGraph* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKRoutingMessageHandler NetGraphMsgHandler_as_RoutingMessageHandler([NativeTypeName("const LDKNetGraphMsgHandler *")] LDKNetGraphMsgHandler* this_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void DirectionalChannelInfo_free(LDKDirectionalChannelInfo this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint DirectionalChannelInfo_get_last_update([NativeTypeName("const LDKDirectionalChannelInfo *")] LDKDirectionalChannelInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void DirectionalChannelInfo_set_last_update([NativeTypeName("LDKDirectionalChannelInfo *")] LDKDirectionalChannelInfo* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte DirectionalChannelInfo_get_enabled([NativeTypeName("const LDKDirectionalChannelInfo *")] LDKDirectionalChannelInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void DirectionalChannelInfo_set_enabled([NativeTypeName("LDKDirectionalChannelInfo *")] LDKDirectionalChannelInfo* this_ptr, [NativeTypeName("bool")] byte val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort DirectionalChannelInfo_get_cltv_expiry_delta([NativeTypeName("const LDKDirectionalChannelInfo *")] LDKDirectionalChannelInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void DirectionalChannelInfo_set_cltv_expiry_delta([NativeTypeName("LDKDirectionalChannelInfo *")] LDKDirectionalChannelInfo* this_ptr, [NativeTypeName("uint16_t")] ushort val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint64_t")]
        public static extern ulong DirectionalChannelInfo_get_htlc_minimum_msat([NativeTypeName("const LDKDirectionalChannelInfo *")] LDKDirectionalChannelInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void DirectionalChannelInfo_set_htlc_minimum_msat([NativeTypeName("LDKDirectionalChannelInfo *")] LDKDirectionalChannelInfo* this_ptr, [NativeTypeName("uint64_t")] ulong val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelUpdate DirectionalChannelInfo_get_last_update_message([NativeTypeName("const LDKDirectionalChannelInfo *")] LDKDirectionalChannelInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void DirectionalChannelInfo_set_last_update_message([NativeTypeName("LDKDirectionalChannelInfo *")] LDKDirectionalChannelInfo* this_ptr, LDKChannelUpdate val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 DirectionalChannelInfo_write([NativeTypeName("const LDKDirectionalChannelInfo *")] LDKDirectionalChannelInfo* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKDirectionalChannelInfo DirectionalChannelInfo_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelInfo_free(LDKChannelInfo this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey ChannelInfo_get_node_one([NativeTypeName("const LDKChannelInfo *")] LDKChannelInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelInfo_set_node_one([NativeTypeName("LDKChannelInfo *")] LDKChannelInfo* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKDirectionalChannelInfo ChannelInfo_get_one_to_two([NativeTypeName("const LDKChannelInfo *")] LDKChannelInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelInfo_set_one_to_two([NativeTypeName("LDKChannelInfo *")] LDKChannelInfo* this_ptr, LDKDirectionalChannelInfo val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKPublicKey ChannelInfo_get_node_two([NativeTypeName("const LDKChannelInfo *")] LDKChannelInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelInfo_set_node_two([NativeTypeName("LDKChannelInfo *")] LDKChannelInfo* this_ptr, LDKPublicKey val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKDirectionalChannelInfo ChannelInfo_get_two_to_one([NativeTypeName("const LDKChannelInfo *")] LDKChannelInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelInfo_set_two_to_one([NativeTypeName("LDKChannelInfo *")] LDKChannelInfo* this_ptr, LDKDirectionalChannelInfo val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelAnnouncement ChannelInfo_get_announcement_message([NativeTypeName("const LDKChannelInfo *")] LDKChannelInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ChannelInfo_set_announcement_message([NativeTypeName("LDKChannelInfo *")] LDKChannelInfo* this_ptr, LDKChannelAnnouncement val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 ChannelInfo_write([NativeTypeName("const LDKChannelInfo *")] LDKChannelInfo* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKChannelInfo ChannelInfo_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RoutingFees_free(LDKRoutingFees this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint RoutingFees_get_base_msat([NativeTypeName("const LDKRoutingFees *")] LDKRoutingFees* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RoutingFees_set_base_msat([NativeTypeName("LDKRoutingFees *")] LDKRoutingFees* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint RoutingFees_get_proportional_millionths([NativeTypeName("const LDKRoutingFees *")] LDKRoutingFees* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void RoutingFees_set_proportional_millionths([NativeTypeName("LDKRoutingFees *")] LDKRoutingFees* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKRoutingFees RoutingFees_new([NativeTypeName("uint32_t")] uint base_msat_arg, [NativeTypeName("uint32_t")] uint proportional_millionths_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKRoutingFees RoutingFees_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 RoutingFees_write([NativeTypeName("const LDKRoutingFees *")] LDKRoutingFees* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeAnnouncementInfo_free(LDKNodeAnnouncementInfo this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint NodeAnnouncementInfo_get_last_update([NativeTypeName("const LDKNodeAnnouncementInfo *")] LDKNodeAnnouncementInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeAnnouncementInfo_set_last_update([NativeTypeName("LDKNodeAnnouncementInfo *")] LDKNodeAnnouncementInfo* this_ptr, [NativeTypeName("uint32_t")] uint val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[3]")]
        public static extern byte** NodeAnnouncementInfo_get_rgb([NativeTypeName("const LDKNodeAnnouncementInfo *")] LDKNodeAnnouncementInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeAnnouncementInfo_set_rgb([NativeTypeName("LDKNodeAnnouncementInfo *")] LDKNodeAnnouncementInfo* this_ptr, LDKThreeBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const uint8_t (*)[32]")]
        public static extern byte** NodeAnnouncementInfo_get_alias([NativeTypeName("const LDKNodeAnnouncementInfo *")] LDKNodeAnnouncementInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeAnnouncementInfo_set_alias([NativeTypeName("LDKNodeAnnouncementInfo *")] LDKNodeAnnouncementInfo* this_ptr, LDKThirtyTwoBytes val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeAnnouncementInfo_set_addresses([NativeTypeName("LDKNodeAnnouncementInfo *")] LDKNodeAnnouncementInfo* this_ptr, [NativeTypeName("LDKCVec_NetAddressZ")] LDKCVecTempl_NetAddress val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKNodeAnnouncement NodeAnnouncementInfo_get_announcement_message([NativeTypeName("const LDKNodeAnnouncementInfo *")] LDKNodeAnnouncementInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeAnnouncementInfo_set_announcement_message([NativeTypeName("LDKNodeAnnouncementInfo *")] LDKNodeAnnouncementInfo* this_ptr, LDKNodeAnnouncement val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 NodeAnnouncementInfo_write([NativeTypeName("const LDKNodeAnnouncementInfo *")] LDKNodeAnnouncementInfo* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKNodeAnnouncementInfo NodeAnnouncementInfo_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeInfo_free(LDKNodeInfo this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeInfo_set_channels([NativeTypeName("LDKNodeInfo *")] LDKNodeInfo* this_ptr, [NativeTypeName("LDKCVec_u64Z")] LDKCVecTempl_u64 val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKRoutingFees NodeInfo_get_lowest_inbound_channel_fees([NativeTypeName("const LDKNodeInfo *")] LDKNodeInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeInfo_set_lowest_inbound_channel_fees([NativeTypeName("LDKNodeInfo *")] LDKNodeInfo* this_ptr, LDKRoutingFees val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKNodeAnnouncementInfo NodeInfo_get_announcement_info([NativeTypeName("const LDKNodeInfo *")] LDKNodeInfo* this_ptr);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NodeInfo_set_announcement_info([NativeTypeName("LDKNodeInfo *")] LDKNodeInfo* this_ptr, LDKNodeAnnouncementInfo val);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKNodeInfo NodeInfo_new([NativeTypeName("LDKCVec_u64Z")] LDKCVecTempl_u64 channels_arg, LDKRoutingFees lowest_inbound_channel_fees_arg, LDKNodeAnnouncementInfo announcement_info_arg);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 NodeInfo_write([NativeTypeName("const LDKNodeInfo *")] LDKNodeInfo* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKNodeInfo NodeInfo_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("LDKCVec_u8Z")]
        public static extern LDKCVecTempl_u8 NetworkGraph_write([NativeTypeName("const LDKNetworkGraph *")] LDKNetworkGraph* obj);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKNetworkGraph NetworkGraph_read(LDKu8slice ser);

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern LDKNetworkGraph NetworkGraph_new();

        [DllImport("", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void NetworkGraph_close_channel_from_update([NativeTypeName("LDKNetworkGraph *")] LDKNetworkGraph* this_arg, [NativeTypeName("uint64_t")] ulong short_channel_id, [NativeTypeName("bool")] byte is_permanent);
    }
}
