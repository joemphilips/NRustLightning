using System;
using System.Text;
using System.Transactions;
using Xunit;
using DotNetLightning.LDK;

namespace DotNetLightning.LDK.Tests
{
    public class ChannelManagerTests
    {
        [Fact]
        public void ShouldHandleSimpleStruct()
        {
            var ffiStruct = Interop.TestOutputStruct.Create(new Interop.FFITestInputInterface().ToFFI());
            ffiStruct.Dispose();

            var test2 = Interop.ffi_test_function_ptr();
            test2.Check();
        }
        
        [Fact]
        public void CanCreateChannelManager()
        {
        }
    }
}