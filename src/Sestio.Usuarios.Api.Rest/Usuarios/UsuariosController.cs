using Microsoft.AspNetCore.Mvc;
using Sestio.Usuarios.App.Services.Usuarios.Requests;
using Sestio.Usuarios.App.Services.Usuarios.Responses;
using Sestio.Usuarios.App.Services.Usuarios.Services;

namespace Sestio.Usuarios.Api.Rest.Usuarios;

[ApiController]
public class UsuariosController
{
    [HttpPost("usuarios")]
    public async Task<UsuarioResponse> CriarUsuarioAsync(
        [FromServices] IUsuarioHandler handler,
        [FromBody] CriarUsuarioRequest request)
    {
        var view = await handler.CriarUsuarioAsync(request);
        return view;
    }

#if DEBUG
    [HttpGet("usuarios")]
    public async Task<List<UsuarioResponse>> ListarUsuarioAsync(
        [FromServices] IUsuarioHandler handler)
    {
        var views = await handler.ListarAsync();
        return views;
    }
#endif
}
