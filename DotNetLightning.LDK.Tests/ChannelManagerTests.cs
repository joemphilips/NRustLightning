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
            var inputStruct = new Interop.FFITestInputInterface().ToFFI();
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
        }
    }
}