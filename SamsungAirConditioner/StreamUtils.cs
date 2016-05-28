using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SamsungAirConditioner
{
    public static class StreamUtils
    {
        public static async Task<string> ReadMessageAsync(Stream stream, Predicate<string> exitPredicate = null)
        {
            int bytes = -1;

            StringBuilder message = new StringBuilder();

            while (bytes != 0)
            {
                byte[] buffer = new byte[1024];

                bytes = await stream.ReadAsync(buffer, 0, buffer.Length);

                Decoder decoder = Encoding.UTF8.GetDecoder();

                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];

                decoder.GetChars(buffer, 0, bytes, chars, 0);

                message.Append(chars);

                Console.WriteLine(message.ToString());

                if (exitPredicate != null && exitPredicate(message.ToString()))
                {
                    break;
                }
            }

            return message.ToString();
        }

        public static async Task SendMessageAsync(Stream stream, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            await stream.WriteAsync(buffer, 0, buffer.Length);

            await stream.FlushAsync();
        }
    }
}