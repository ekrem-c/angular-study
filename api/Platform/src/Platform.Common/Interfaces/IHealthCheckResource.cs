using System.Threading.Tasks;

namespace Platform.Common
{
    public interface IHealthCheckResource
    {
        Task<HealthCheckResponse> CheckHealthAsync();
    }
}