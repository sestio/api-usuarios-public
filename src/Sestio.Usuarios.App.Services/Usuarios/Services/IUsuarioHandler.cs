using Sestio.Usuarios.App.Services.Usuarios.Requests;
using Sestio.Usuarios.App.Services.Usuarios.Responses;

namespace Sestio.Usuarios.App.Services.Usuarios.Services;

public interface IUsuarioHandler
{
    Task<UsuarioResponse> CriarUsuarioAsync(CriarUsuarioRequest request);
    Task<List<UsuarioResponse>> ListarAsync();
}
