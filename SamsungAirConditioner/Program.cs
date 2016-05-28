using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamsungAirConditioner
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionFactory = new SimpleConnectionFactory("192.168.1.2");
            var ac = new AirConditioner(connectionFactory);
            Task.Factory.StartNew(async () =>
            {
              

                var token = new Token("8852e023-ddfd-4ae4-8f55-dccd0a358246"); //await TokenRequester.GetTokenAsync(connectionFactory);

                Console.WriteLine(token.Id);

           

                await ac.Login(token);

                Console.WriteLine($"Connected to AC with token {token.Id}");

            });

            while (true)
            {
                var val = Console.ReadLine();
                Task.Factory.StartNew(async () => await ac.SetTemperature(int.Parse(val)));
            }
        }
    }
}
