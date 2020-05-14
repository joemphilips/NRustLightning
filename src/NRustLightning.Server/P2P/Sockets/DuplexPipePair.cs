using System.IO.Pipelines;

namespace NRustLightning.Server.P2P.Sockets
{
    public readonly struct DuplexPipePair
    {
        public IDuplexPipe Transport { get; }
        public IDuplexPipe Application { get; }

        public DuplexPipePair(IDuplexPipe transport, IDuplexPipe application)
        {
            Transport = transport;
            Application = application;
        }
        
        public static DuplexPipePair CreateConnectionPair(PipeOptions inputOptions, PipeOptions outputOptions)
        {
            var input = new Pipe(inputOptions);
            var output = new Pipe(outputOptions);
            
            var transportToApp = new DuplexPipe(output.Reader, input.Writer);
            var appToTransport = new DuplexPipe(input.Reader, output.Writer);
            return new DuplexPipePair(appToTransport, transportToApp);
        }
    }
}