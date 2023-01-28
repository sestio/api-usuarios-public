using Sestio.Commons.Domain;
using Sestio.Commons.JsonWebTokens.Services.PeerJwts;
using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Sessoes.Entities;
using Sestio.Usuarios.Domain.Sessoes.Values;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.Domain.Sessoes.Services;

public sealed class GerenciadorSessao
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISessaoRepository _sessaoRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPeerJwtBuilder _peerJwtBuilder;
    private readonly SessaoOptions _sessaoOptions;

    public GerenciadorSessao(IUnitOfWork unitOfWork,
                             IUsuarioRepository usuarioRepository,
                             ISessaoRepository sessaoRepository,
                             IRefreshTokenRepository refreshTokenRepository,
                             IPeerJwtBuilder peerJwtBuilder,
                             SessaoOptions sessaoOptions)
    {
        _unitOfWork = unitOfWork;
        _usuarioRepository = usuarioRepository;
        _sessaoRepository = sessaoRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _peerJwtBuilder = peerJwtBuilder;
        _sessaoOptions = sessaoOptions;
    }

    public async Task<Sessao> CriarSessaoAsync(Usuario usuario)
    {
        var sessao = new Sessao(usuario, _sessaoOptions.DuracaoSessao);

        _sessaoRepository.Add(sessao);
        await _unitOfWork.SaveChangesAsync();

        return sessao;
    }

    public async Task<ResultadoDadosRenovacaoAcesso> ObterSessaoParaRenovacaoAcessoAsync(string requestRefreshToken)
    {
        var token = await _refreshTokenRepository.ObterPorTokenAsync(requestRefreshToken);
        if (token == null) return ResultadoDadosRenovacaoAcesso.Criar(null);

        var sessao = await ObterSessaoValidadaAsync(token);
        await _unitOfWork.SaveChangesAsync();

        var result = ResultadoDadosRenovacaoAcesso.Criar(sessao);
        return result;
    }

    private async Task<Sessao?> ObterSessaoValidadaAsync(RefreshToken token)
    {
        var sessao = await _sessaoRepository.ObterPorTokenAsync(token);

        if (!sessao.IsValidaParaRenovacao())
        {
            await InvalidarTokenAtivoSeExistirAsync(sessao);
            return sessao;
        }

        if (!token.IsValidoParaRenovacao())
        {
            await InvalidarSessaoAsync(sessao);
            return sessao;
        }

        sessao.AtualizarSituacaoValidade();
        if (sessao.Situacao == SituacaoSessao.Expirada)
            token.Invalidar();
        return sessao;
    }

    private async Task InvalidarTokenAtivoSeExistirAsync(Sessao sessao)
    {
        var token = await _refreshTokenRepository.ObterAtivoPorSessaoAsync(sessao);
        token?.Invalidar();
    }

    private async Task InvalidarSessaoAsync(Sessao sessao)
    {
        await InvalidarTokenAtivoSeExistirAsync(sessao);
        sessao.Revogar();
    }

    public async Task<DadosAcesso> RenovarAcessoAsync(Sessao sessao, IGeradorRefreshToken geradorRefreshToken)
    {
        var usuario = await _usuarioRepository.ObterPorSessaoAsync(sessao);

        var refreshTokenAtual = await _refreshTokenRepository.ObterAtivoPorSessaoAsync(sessao);
        var novoRefreshToken = CriarRefreshToken(sessao, geradorRefreshToken);

        refreshTokenAtual?.Substituir();
        novoRefreshToken.Ativar();

        _refreshTokenRepository.Add(novoRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        var accessToken = CriarAccessToken(usuario);
        var result = new DadosAcesso(usuario, accessToken, novoRefreshToken);
        return result;
    }

    private AccessToken CriarAccessToken(Usuario usuario)
    {
        var accessJwtOptions = new PeerIdJwtOptions
        {
            TenantId = usuario.IdTenant.ToString(),
            UserId = usuario.Id.ToString(),
            UserName = usuario.Nome,
            UserEmail = usuario.Email,
            TimeToLive = _sessaoOptions.DuracaoAccessToken
        };
        var accessJwt = _peerJwtBuilder.CreateToken(accessJwtOptions);

        var result = new AccessToken(accessJwt.Value, accessJwtOptions.TimeToLive);
        return result;
    }

    private static RefreshToken CriarRefreshToken(Sessao sessao, IGeradorRefreshToken geradorRefreshToken)
    {
        var tokenValue = geradorRefreshToken.Gerar(sessao);
        return new RefreshToken(sessao, tokenValue);
    }
}
