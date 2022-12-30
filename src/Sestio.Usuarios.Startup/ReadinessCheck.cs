using Microsoft.Extensions.Diagnostics.HealthChecks;
using Sestio.Commons.Startup.HealthChecks;

namespace Sestio.Usuarios.Startup;

public sealed class ReadinessCheck : IReadinessCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        // TODO: Esperar pela aplicação ter mais dependências antes de colocar qualquer lógica aqui
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}
