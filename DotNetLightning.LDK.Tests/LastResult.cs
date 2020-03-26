using System;
using System.Text;
using System.Transactions;
using Xunit;
using DotNetLightning.LDK;

namespace DotNetLightning.LDK.Tests
{
    public class LastResultTests
    {
        [Fact]
        public void NativeErrorsShouldBecomeExceptions()
        {
            var nativeException = Assert.Throws<Exception>(() => Interop.ffi_test_error());
            Assert.Equal("Native storage failed (InternalError), A test error.", nativeException.Message);
        }
    }
}
