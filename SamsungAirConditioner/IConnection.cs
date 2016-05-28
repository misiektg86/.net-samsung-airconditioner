using System.Net.Security;
using System.Threading.Tasks;

namespace SamsungAirConditioner
{
    public interface IConnectionFactory
    {
        Task<SslStream> CreateConnectionAsync();
    }
}