using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SamsungAirConditioner
{
    public sealed class AirConditioner : IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;

        private Stream _stream;

        private readonly Thread _infoThread;

        private readonly Random _random = new Random();

        private const string Duid = "F8042E300BFE";

        public AirConditioner(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;

            _infoThread = new Thread(() =>
           {
               while (true)
               {
                   StreamUtils.ReadMessageAsync(_stream, i => i.Contains("?xml")).Wait();
               }
           });
        }

        public async Task Login(Token token)
        {
            _stream = await _connectionFactory.CreateConnectionAsync();

            await StreamUtils.ReadMessageAsync(_stream, i => i.Contains("InvalidateAccount"));

            await StreamUtils.SendMessageAsync(_stream, "<Request Type=\"AuthToken\"><User Token=\"" + token.Id + "\" /></Request>\r\n");

            await StreamUtils.ReadMessageAsync(_stream, i => i.Contains("Response Type=\"AuthToken\" Status=\"Okay\""));

            _infoThread.Start();
        }

        public async Task OnOff(PowerSwitch powerSwitch)
        {
            await SendCommand(Commands.AC_FUN_POWER, powerSwitch.ToString());
        }

        public async Task SetTemperature(Temperature temp)
        {
            await SendCommand(Commands.AC_FUN_TEMPSET, ((int)temp).ToString());
        }

        public async Task SetFanSpeed(FanSpeed speed)
        {
            await SendCommand(Commands.AC_FUN_WINDLEVEL, speed.ToString());
        }

        public async Task SetOptions(Options options)
        {
            await SendCommand(Commands.AC_FUN_COMODE, options.ToString());
        }

        public async Task SetMode(Mode mode)
        {
            await SendCommand(Commands.AC_FUN_OPMODE, mode.ToString());
        }

        public async Task RequestDeviceStatus()
        {
            await StreamUtils.SendMessageAsync(_stream, "<Request Type=\"DeviceState\" DUID=\"" + Duid + "\"></Request>\r\n");
        }

        private async Task SendCommand(string command, string value)
        {
            await StreamUtils.SendMessageAsync(_stream, "<Request Type=\"DeviceControl\"><Control CommandID=\"cmd" + _random.Next() + "\" DUID=\"" + Duid + "\"><Attr ID=\"" + command + "\" Value=\"" + value + "\" /></Control></Request>\r\n");
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        private static class Commands
        {
            internal const string AC_FUN_WINDLEVEL = "AC_FUN_WINDLEVEL";

            internal const string AC_FUN_TEMPSET = "AC_FUN_TEMPSET";

            internal const string AC_FUN_COMODE = "AC_FUN_COMODE";

            internal const string AC_FUN_POWER = "AC_FUN_POWER";

            internal const string AC_FUN_OPMODE = "AC_FUN_OPMODE";
        }
    }

    public enum Mode
    {
        Auto,
        Cool,
        Dry,
        Wind,
        Heat
    }

    public enum PowerSwitch
    {
        On,
        Off
    }

    public enum Options
    {
        Off,
        Quiet,
        Sleep,
        Smart,
        SoftCool,
        TurboMode,
        WindMode1,
        WindMode2,
        WindMode3
    }

    public enum FanSpeed
    {
        Auto,
        Low,
        Mid,
        High,
        Turbo
    }
}