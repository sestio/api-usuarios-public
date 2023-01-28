using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sestio.Usuarios.App.Services.Sessoes.Requests;
using Sestio.Usuarios.App.Services.Sessoes.Responses;
using Sestio.Usuarios.App.Services.Sessoes.Services;

namespace Sestio.Usuarios.Api.Rest.Sessoes;

[ApiController]
public class SessoesController : ControllerBase
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
        var refreshToken = CookieHelper.ReadRefreshTokenCookie(Request.Cookies);
        var request = new RenovarAcessoRequest { RefreshToken = refreshToken };
        var response = await handler.RenovarAcessoAsync(request);

        if (!response.Sucesso)
            return Erro(response);

        return Sucesso(response);
    }

    private ActionResult<SessaoView?> Sucesso(SessaoResponse sessaoResponse)
    {
        CookieHelper.WriteRefreshTokenCookie(Response.Cookies, sessaoResponse.RefreshToken!);
        var view = SessaoViewMapper.Map(sessaoResponse);
        return Ok(view);
    }

    private ActionResult<SessaoView?> Erro(SessaoResponse sessaoResponse)
    {
        CookieHelper.RemoveRefreshTokenCookie(Response.Cookies);

        return sessaoResponse.MotivoFalha switch
        {
            MotivoFalhaSessaoResponse.CredenciaisInvalidas => new JsonResult(new { Message = "Credenciais inválidas" })
            {
                StatusCode = StatusCodes.Status400BadRequest
            },
            MotivoFalhaSessaoResponse.SessaoInvalida => new JsonResult(new { Message = "Operação não autorizada" })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            },
            _ => new JsonResult(new { Message = "Erro não esperado" })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            }
        };
    }
}
