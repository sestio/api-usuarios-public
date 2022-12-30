using Sestio.Usuarios.App.Services.Usuarios.Commands;
using Sestio.Usuarios.App.Services.Usuarios.Views;

namespace Sestio.Usuarios.App.Services.Usuarios.Services;

public interface IUsuarioService
{
    Task<UsuarioView> CriarUsuarioAsync(CriarUsuarioCommand command);
    Task<List<UsuarioView>> ListarAsync();
}
