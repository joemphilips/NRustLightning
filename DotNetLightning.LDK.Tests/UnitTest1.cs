using System;
using System.Text;
using System.Transactions;
using Xunit;
using DotNetLightning.LDK;

namespace DotNetLightning.LDK.Tests
{
    static class Helper
    {
        private unsafe CreateChannelManagerWithNullPointer()
        {

        }
    }

    public class UnitTest1
    {
        [Fact]
        public void CanGetErrorMessage()
        {
            var noError = Interop.last_error_message();
            Assert.Empty(noError);
            var seed = new byte[] {0,1,2,3};
            Interop.create_ffi_channel_manager()
        }
    }
}