using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SamsungAirConditioner
{
    internal class SimpleConnectionFactory : IConnectionFactory
    {
        private readonly string _acAddress;
        private const int AcPort = 2878;

        public SimpleConnectionFactory(string acAddress)
        {
            _acAddress = acAddress;
        }

        public async Task<SslStream> CreateConnectionAsync()
        {
            TcpClient tcpClient = new TcpClient(_acAddress, AcPort);

            X509Certificate2Collection certs = new X509Certificate2Collection();

            certs.Import(Resource.ac14k_m);

            SslStream sslStream = new SslStream(tcpClient.GetStream(), false, (sender, certificate, chain, errors) => true, null);

            Debug.WriteLine($"Connecting to device {_acAddress}");

            await sslStream.AuthenticateAsClientAsync(_acAddress, certs, SslProtocols.Tls, false);

            Debug.WriteLine($"Connected to device {_acAddress}");

            return sslStream;
        }
    }
}