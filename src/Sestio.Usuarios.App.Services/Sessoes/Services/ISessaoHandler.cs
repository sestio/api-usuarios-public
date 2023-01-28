using Sestio.Usuarios.App.Services.Sessoes.Requests;
using Sestio.Usuarios.App.Services.Sessoes.Responses;

namespace Sestio.Usuarios.App.Services.Sessoes.Services;

public interface ISessaoHandler
{
    Task<SessaoResponse> IniciarSessaoAsync(IniciarSessaoRequest request);
    Task<SessaoResponse> RenovarAcessoAsync(RenovarAcessoRequest request);
}
