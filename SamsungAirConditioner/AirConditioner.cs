using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamsungAirConditioner
{
    public class AirConditioner : IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;

        private Stream _stream;

        public AirConditioner(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task Login(Token token)
        {
            _stream = await _connectionFactory.CreateConnectionAsync();

            await StreamUtils.ReadMessageAsync(_stream, i => i.Contains("InvalidateAccount"));

            await StreamUtils.SendMessageAsync(_stream, "<Request Type=\"AuthToken\"><User Token=\"" + token.Id + "\" /></Request>\r\n");

            await StreamUtils.ReadMessageAsync(_stream, i => i.Contains("Response Type=\"AuthToken\" Status=\"Okay\""));

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await StreamUtils.ReadMessageAsync(_stream, i => i.Contains("?xml"));
                }
            });
        }

        public async Task SetTemperature(int temp)
        {
            if (temp < 16)
            {
                temp = 16;
            }

            if (temp > 30)
            {
                temp = 30;
            }

            await StreamUtils.SendMessageAsync(_stream, "<Request Type=\"DeviceControl\"><Control CommandID=\"cmd" + _random.Next() + "\" DUID=\"" + Duid + "\"><Attr ID=\"AC_FUN_TEMPSET\" Value=\"" + temp.ToString() + "\" /></Control></Request>\r\n");
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        private Random _random = new Random();

        private const string Duid = "F8042E300BFE";
    }
}