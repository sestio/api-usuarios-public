using Sestio.Commons.Domain;
using Sestio.Commons.JsonWebTokens.Services.PeerJwts;
using Sestio.Commons.Validation.Services;
using Sestio.Usuarios.App.Services.Sessoes.Requests;
using Sestio.Usuarios.App.Services.Sessoes.Responses;
using Sestio.Usuarios.App.Services.Sessoes.Services;
using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Sessoes.Entities;
using Sestio.Usuarios.Domain.Sessoes.Services;
using Sestio.Usuarios.Domain.Usuarios.Entities;
using Sestio.Usuarios.Domain.Usuarios.Services;

namespace Sestio.Usuarios.App.Handlers.Sessoes;

public class SessaoHandler : ISessaoHandler
{
    private readonly GerenciadorUsuario _gerenciadorUsuario;
    private readonly IGeradorRefreshToken _geradorRefreshToken;
    private readonly INotificationBag _notifications;
    private readonly GerenciadorSessao _gerenciadorSessao;

    public SessaoHandler(IUnitOfWork unitOfWork,
                         IUsuarioRepository usuarioRepository,
                         ISessaoRepository sessaoRepository,
                         IRefreshTokenRepository refreshTokenRepository,
                         IUserPasswordHasher<Usuario> userPasswordHasher,
                         IPeerJwtBuilder peerJwtBuilder,
                         IGeradorRefreshToken geradorRefreshToken,
                         SessaoOptions sessaoOptions,
                         INotificationBag notifications)
    {
        _gerenciadorUsuario = new GerenciadorUsuario(usuarioRepository, userPasswordHasher);
        _geradorRefreshToken = geradorRefreshToken;
        _notifications = notifications;
        _gerenciadorSessao = new GerenciadorSessao(
            unitOfWork,
            usuarioRepository,
            sessaoRepository,
            refreshTokenRepository,
            peerJwtBuilder,
            sessaoOptions);
    }

    public async Task<SessaoResponse> IniciarSessaoAsync(IniciarSessaoRequest request)
    {
        var credenciais = new CredenciaisUsuario(request.Email, request.Senha);
        var resultadoAutenticacao = await _gerenciadorUsuario.ObterUsuarioAutenticadoAsync(credenciais);

        if (!resultadoAutenticacao.Sucesso)
        {
            _notifications.AddError("INVALID_CREDENTIALS", "Credenciais inválidas");
            return SessaoResponseMapper.Invalido(MotivoFalhaSessaoResponse.CredenciaisInvalidas);
        }

        var sessao = await _gerenciadorSessao.CriarSessaoAsync(resultadoAutenticacao.Usuario!);

        var dadosAcesso = await _gerenciadorSessao.RenovarAcessoAsync(sessao, _geradorRefreshToken);

        var result = SessaoResponseMapper.Valido(
            dadosAcesso.Usuario,
            dadosAcesso.AccessToken,
            dadosAcesso.RefreshToken);
        return result;
    }

    public async Task<SessaoResponse> RenovarAcessoAsync(RenovarAcessoRequest request)
    {
        var resultadoSessao = await _gerenciadorSessao.ObterSessaoParaRenovacaoAcessoAsync(request.RefreshToken);

        if (!resultadoSessao.Sucesso)
            return SessaoResponseMapper.Invalido(MotivoFalhaSessaoResponse.SessaoInvalida);

        var dadosAcesso = await _gerenciadorSessao.RenovarAcessoAsync(resultadoSessao.Sessao!, _geradorRefreshToken);

        var result = SessaoResponseMapper.Valido(
            dadosAcesso.Usuario,
            dadosAcesso.AccessToken,
            dadosAcesso.RefreshToken);
        return result;
    }
}
