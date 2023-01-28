using Microsoft.Extensions.Diagnostics.HealthChecks;
using Sestio.Commons.Startup.HealthChecks;

namespace Sestio.Usuarios.Startup;

public sealed class ReadinessCheck : IReadinessCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}
