using System;
using System.Text;
using System.Transactions;
using Xunit;
using DotNetLightning.LDK;
using DotNetLightning.LDK.Adaptors;
using DotNetLightning.LDK.Tests.Utils;

namespace DotNetLightning.LDK.Tests
{
    public class ChannelManagerTests
    {
        [Fact]
        public void ShouldHandleSimpleStruct()
        {
            // this object must kept alive until the ffi code finish calling delegate.
            // Since object holds the reference to the delegate, if this has been removed by GC
            // the pointer on the rust side wil 
            var inputInterface = new Interop.FFITestInputInterface();
            var inputStruct = inputInterface.ToFFI();
            var ffiStruct = Interop.TestOutputStruct.Create(inputStruct);
            ffiStruct.Dispose();

            var test2 = Interop.ffi_test_function_ptr();
            test2.Check();

            var test3 = Interop.ffi_simple_nested_struct();
            var test4 = Interop.ffi_simple_nested_struct_with_return_value();
            test4.Check();
        }
        
        [Fact]
        public void CanCreateChannelManager()
        {
            var logger = new TestLogger();
            var broadcaster = new TestBroadcaster();
            var feeEstiamtor = new TestFeeEstimator();
            var chainWatchInterface = new TestChainWatchInterface();
            var seed = new byte[]{ 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }.AsSpan();
            var n = Network.TestNet;
            var channelManager = ChannelManager.Create(seed, in n, in TestUserConfig.Default, chainWatchInterface, logger, broadcaster, feeEstiamtor, 400000);
            channelManager.Dispose();
        }
    }
}