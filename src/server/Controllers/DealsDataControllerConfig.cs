using System.Text;

namespace DTDemo.Server.Controllers
{
    public class DealsDataControllerConfig
    {
        public DealsDataControllerConfig(Encoding csvFileEncoding, int clientBufferSize)
        {
            CsvFileEncoding = csvFileEncoding;
            ClientBufferSize = clientBufferSize;
        }

        public Encoding CsvFileEncoding { get; }
        public int ClientBufferSize { get; }
    }
}
