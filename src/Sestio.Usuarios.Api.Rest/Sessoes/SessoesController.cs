using Microsoft.AspNetCore.Mvc;
using Sestio.Commons.Api.Rest.Controllers;
using Sestio.Usuarios.App.Services.Sessoes.Requests;
using Sestio.Usuarios.App.Services.Sessoes.Responses;
using Sestio.Usuarios.App.Services.Sessoes.Services;

namespace Sestio.Usuarios.Api.Rest.Sessoes;

[ApiController]
public class SessoesController : SestioController
{
    [HttpPost("sessoes")]
    public async Task<ActionResult<SessaoView?>> IniciarSessaoAsync(
        [FromServices] ISessaoHandler handler,
        [FromBody] IniciarSessaoRequest request)
    {
        var response = await handler.IniciarSessaoAsync(request);

        if (!response.Sucesso)
            return Erro(response);

        return Sucesso(response);
    }

    [HttpGet("sessoes")]
    public async Task<ActionResult<SessaoView?>> IniciarSessaoAsync(
        [FromServices] ISessaoHandler handler)
    {
        var refreshToken = CookieHelper.ReadRefreshTokenCookie(HttpContext.Request.Cookies);
        var request = new RenovarAcessoRequest { RefreshToken = refreshToken };
        var response = await handler.RenovarAcessoAsync(request);

        if (!response.Sucesso)
            return Erro(response);

        return Sucesso(response);
    }

    private ActionResult<SessaoView?> Sucesso(SessaoResponse sessaoResponse)
    {
        CookieHelper.WriteRefreshTokenCookie(HttpContext.Response.Cookies, sessaoResponse.RefreshToken!);
        var view = SessaoViewMapper.Map(sessaoResponse);
        return Ok(view);
    }

    private ActionResult Erro(SessaoResponse sessaoResponse)
    {
        CookieHelper.RemoveRefreshTokenCookie(HttpContext.Response.Cookies);

        return sessaoResponse.MotivoFalha switch
        {
            MotivoFalhaSessaoResponse.CredenciaisInvalidas => BadRequest(),
            MotivoFalhaSessaoResponse.SessaoInvalida =>  Unauthorized(),
            _ => ServerError()
        };
    }
}
