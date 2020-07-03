using System.IO.Pipelines;
using NRustLightning.Server.P2P.Sockets;

namespace NRustLightning.Server.P2P
{
    public class DuplexPipe : IDuplexPipe
    {
        public PipeReader Input { get; }
        public PipeWriter Output { get; }

        public DuplexPipe(PipeReader reader, PipeWriter writer)
        {
            Input = reader;
            Output = writer;
        }
        public static DuplexPipePair CreateConnection(PipeOptions inputOptions, PipeOptions outputOptions)
        {
            var input = new Pipe(inputOptions);
            var output = new Pipe(outputOptions);
            var transportToApplication = new DuplexPipe(output.Reader, input.Writer);
            var applicationToTransport = new DuplexPipe(input.Reader, output.Writer);
            return new DuplexPipePair(applicationToTransport, transportToApplication);
        }

    }
}