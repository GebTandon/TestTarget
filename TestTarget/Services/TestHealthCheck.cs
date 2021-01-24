using Microsoft.Extensions.Diagnostics.HealthChecks;

using System.Threading;
using System.Threading.Tasks;

namespace TestTarget.Services
{
    public class TestHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var healthCheckResultHealthy = true;

            return healthCheckResultHealthy
                ? Task.FromResult(HealthCheckResult.Healthy("A healthy result."))
                : Task.FromResult(HealthCheckResult.Unhealthy("An unhealthy result."));
        }
    }
    public class TestHealthCheckv2 : IHealthCheck
    {
        private int _delayInmSecs;

        public TestHealthCheckv2(int delayInmSecs = 100)
        {
            _delayInmSecs = delayInmSecs;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var healthCheckResultHealthy = true;
            await Task.Delay(_delayInmSecs);
            return await (healthCheckResultHealthy
                ? Task.FromResult(HealthCheckResult.Healthy("A healthy result."))
                : Task.FromResult(HealthCheckResult.Unhealthy("An unhealthy result.")));
        }
    }
}
