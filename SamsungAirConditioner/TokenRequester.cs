using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamsungAirConditioner
{
    public class TokenRequester
    {
        public static async Task<Token> GetTokenAsync(IConnectionFactory connectionFactory)
        {
            using (SslStream stream = await connectionFactory.CreateConnectionAsync())
            {
                var msg = await StreamUtils.ReadMessageAsync(stream, i => i.Contains("InvalidateAccount"));

                await StreamUtils.SendMessageAsync(stream, "<Request Type=\"GetToken\" />\r\n");

                await StreamUtils.ReadMessageAsync(stream, i => i.Contains("<?xml version=\"1.0\" encoding=\"utf-8\" ?><Response Type=\"GetToken\" Status=\"Ready\"/>\r\n"));

                msg = await StreamUtils.ReadMessageAsync(stream, i => i.Contains("<Update Type=\"GetToken\" Status=\"Completed\"") || i.Contains("Response Status=\"Fail\" Type=\"Authenticate\" ErrorCode=\"301\""));

                if (msg.Contains("Response Status=\"Fail\" Type=\"Authenticate\" ErrorCode=\"301\""))
                {
                    return null;
                }

                Regex regex = new Regex(@"(?<=\bToken="")[^""]*");
                Match match = regex.Match(msg);
                string token = match.Value;

                return new Token(token);
            }
        }
    }
}