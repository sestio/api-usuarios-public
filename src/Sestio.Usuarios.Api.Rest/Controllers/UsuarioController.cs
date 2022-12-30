using Microsoft.AspNetCore.Mvc;
using Sestio.Usuarios.App.Services.Usuarios.Commands;
using Sestio.Usuarios.App.Services.Usuarios.Services;
using Sestio.Usuarios.App.Services.Usuarios.Views;

namespace Sestio.Usuarios.Api.Rest.Controllers;

[ApiController]
public class UsuarioController
{
    [HttpPost("usuarios")]
    public async Task<UsuarioView> CriarUsuarioAsync(
        [FromServices] IUsuarioService service,
        [FromBody] CriarUsuarioCommand command)
    {
        var view = await service.CriarUsuarioAsync(command);
        return view;
    }

#if DEBUG
    [HttpGet("usuarios")]
    public async Task<List<UsuarioView>> ListarUsuarioAsync(
        [FromServices] IUsuarioService service)
    {
        var views = await service.ListarAsync();
        return views;
    }
#endif
}
