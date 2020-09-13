using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;

namespace NRustLightning.Binding.Tests
{
    public class UnitTest1
    {
        public struct NodeMonitors
        {
            public Mutex mut;
            public List<(Binding.LDKOutPoint, LDKChannelMonitor)> mons;
            public LDKChainWatchInterfaceUtil watchUtil;
            public LDKLogger logger;
        }
        
        [Fact]
        public unsafe void Test1()
        {
            var nodeSeed = new byte[32];
            var net = LDKNetwork.LDKNetwork_Testnet;
            
            var logger = new LDKLogger();
            Log log = (thisArg, record) => {};
            logger.log = (IntPtr)Unsafe.AsPointer(ref log);

            GetEstSatPer1000Weight getEstSatPer1000Weight = (thisArg, confirmationTarget) =>
            {
                if (confirmationTarget == LDKConfirmationTarget.LDKConfirmationTarget_Background)
                {
                    return 253;
                }
                else
                {
                    return 507;
                }
            };
            var feeEst = new LDKFeeEstimator();
            feeEst.get_est_sat_per_1000_weight = (IntPtr)Unsafe.AsPointer(ref getEstSatPer1000Weight);
            
            var chain1 = Methods.ChainWatchInterfaceUtil_new(net);
            var c1 = (LDKChainWatchInterfaceUtil*)Unsafe.AsPointer(ref chain1);
            var blocks1 = Methods.BlockNotifier_new(Methods.ChainWatchInterfaceUtil_as_ChainWatchInterface(c1));
            
            var mon1 = new LDKManyChannelMonitor();
            var mon1ThisArg = new NodeMonitors()
            {
                watchUtil = chain1,
            };
            var mon1ThisArgHandle = GCHandle.Alloc(mon1ThisArg);
            mon1.this_arg = Unsafe.AsPointer(ref mon1ThisArgHandle);
            
            
            var config = Methods.UserConfig_default();
            var cm = Methods.ChannelManager_new(net, feeEst, );
            LDKCVecTempl_ChannelDetails details = Methods.ChannelManager_list_channels();
            var d = details.data;
            Methods.ChannelDetails_get_counterparty_features(d);
            
            mon1ThisArgHandle.Free();
        }
    }
}