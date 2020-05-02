using System;
using System.IO.Pipelines;

namespace NRustLightning.Server
{
    public class Connection
    {
        public IDuplexPipe Transport { get; }
        public IDuplexPipe Application { get; }
        public Connection(IDuplexPipe transport, IDuplexPipe application)
        {
            Transport = transport ?? throw new ArgumentNullException(nameof(transport));
            Application = application ?? throw new ArgumentNullException(nameof(application));
        }
    }
    public class DuplexPipe : IDuplexPipe
    {
        public PipeReader Input { get; }
        public PipeWriter Output { get; }

        public DuplexPipe(PipeReader reader, PipeWriter writer)
        {
            Input = reader;
            Output = writer;
        }
        public static Connection CreateConnection(PipeOptions inputOptions, PipeOptions outputOptions)
        {
            var input = new Pipe(inputOptions);
            var output = new Pipe(outputOptions);
            var transportToApplication = new DuplexPipe(output.Reader, input.Writer);
            var applicationToTransport = new DuplexPipe(input.Reader, output.Writer);
            return new Connection(applicationToTransport, transportToApplication);
        }

    }
}