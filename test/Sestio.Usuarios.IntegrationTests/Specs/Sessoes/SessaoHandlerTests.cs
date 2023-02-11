using FluentAssertions;
using Sestio.Commons.Infra.EntityFramework;
using Sestio.Commons.JsonWebTokens.Core.PeerJwts;
using Sestio.Commons.Validation.Core;
using Sestio.Usuarios.App.Handlers.Sessoes;
using Sestio.Usuarios.App.Services.Sessoes.Requests;
using Sestio.Usuarios.App.Services.Sessoes.Responses;
using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Sessoes.Entities;
using Sestio.Usuarios.Domain.Usuarios.Entities;
using Sestio.Usuarios.Infra.EntityFramework;
using Sestio.Usuarios.Infra.Repositories.Sessoes;
using Sestio.Usuarios.Infra.Repositories.Usuarios;
using Sestio.Usuarios.IntegrationTests.Base.Factories;
using Sestio.Usuarios.IntegrationTests.Base.Helpers;
using Sestio.Usuarios.IntegrationTests.Base.Stubs;
using Sestio.Usuarios.TestLib.Factories;

namespace Sestio.Usuarios.IntegrationTests.Specs.Handlers.Sessoes;

public sealed class SessaoHandlerTests : IDisposable
{
    private readonly DbHelper _dbHelper;

    private const double DuracaoSessaoSegundos = 120;
    private const double DuracaoAccessTokenSegundos = 60;

    public SessaoHandlerTests()
    {
        _dbHelper = new DbHelper(nameof(SessaoHandlerTests));
        _dbHelper.CreateDbContext().Database.EnsureCreated();
    }

    public void Dispose()
    {
        _dbHelper.CreateDbContext().Database.EnsureDeleted();
    }


    [Fact]
    public async Task Rejeita_autenticacao_para_email_desconhecido()
    {
        var passwordHasher = TestUserPasswordHasherFactory.Criar();
        var handler = CriarSessaoHandler(passwordHasher);

        var response = await handler.IniciarSessaoAsync(new IniciarSessaoRequest
        {
            Email = "unknown@example.com",
            Senha = "foobar"
        });

        await AssertCredenciaisInvalidasAsync(response);
    }

    [Fact]
    public async Task Rejeita_autenticacao_para_senha_invalida()
    {
        var passwordHasher = TestUserPasswordHasherFactory.Criar();
        var (usuario, dto) = TestUsuarioFactory.Criar(passwordHasher);
        await _dbHelper.InsertAsync(usuario);

        var handler = CriarSessaoHandler(passwordHasher);

        var response = await handler.IniciarSessaoAsync(new IniciarSessaoRequest
        {
            Email = dto.Email,
            Senha = $"--{dto.Senha}--"
        });

        await AssertCredenciaisInvalidasAsync(response);
    }


    [Fact]
    public async Task Rejeita_acesso_para_sessao_expirada()
    {
        var passwordHasher = TestUserPasswordHasherFactory.Criar();
        var (usuario, _) = TestUsuarioFactory.Criar(passwordHasher);
        var sessao = TestSessaoFactory.CriarExpirada(usuario);
        var tokenOriginal = TestRefreshTokenFactory.CriarAtivo(sessao);
        await _dbHelper.InsertAsync(usuario, sessao, tokenOriginal);
        var handler = CriarSessaoHandler();

        var response = await handler.RenovarAcessoAsync(new RenovarAcessoRequest
        {
            RefreshToken = tokenOriginal.Token
        });

        var tokens = await _dbHelper.AllAsync<RefreshToken>(p => p.IdSessao == sessao.Id);
        var sessaoNoBanco = await _dbHelper.SingleAsync<Sessao>(p => p.Id == sessao.Id);
        var tokenNoBanco = tokens.Should().ContainSingle(p => p.IdSessao == sessao.Id).Subject;

        sessaoNoBanco.Situacao.Should().Be(SituacaoSessao.Expirada);
        tokenNoBanco.Situacao.Should().Be(SituacaoRefreshToken.Invalido);
        AssertResponseAcessoInvalido(response, MotivoFalhaSessaoResponse.SessaoInvalida);
    }

    [Fact]
    public async Task Rejeita_acesso_para_sessao_revogada()
    {
        var passwordHasher = TestUserPasswordHasherFactory.Criar();
        var (usuario, _) = TestUsuarioFactory.Criar(passwordHasher);
        var sessao = TestSessaoFactory.CriarRevogada(usuario);
        var tokenOriginal = TestRefreshTokenFactory.CriarAtivo(sessao);
        await _dbHelper.InsertAsync(usuario, sessao, tokenOriginal);
        var handler = CriarSessaoHandler();

        var response = await handler.RenovarAcessoAsync(new RenovarAcessoRequest
        {
            RefreshToken = tokenOriginal.Token
        });

        var tokens = await _dbHelper.AllAsync<RefreshToken>(p => p.IdSessao == sessao.Id);
        var sessaoNoBanco = await _dbHelper.SingleAsync<Sessao>(p => p.Id == sessao.Id);
        var tokenNoBanco = tokens.Should().ContainSingle(p => p.IdSessao == sessao.Id).Subject;

        sessaoNoBanco.Situacao.Should().Be(SituacaoSessao.Revogada);
        tokenNoBanco.Situacao.Should().Be(SituacaoRefreshToken.Invalido);
        AssertResponseAcessoInvalido(response, MotivoFalhaSessaoResponse.SessaoInvalida);
    }


    [Fact]
    public async Task Inicia_sessao_quando_as_credenciais_informadas_sao_validas()
    {
        var passwordHasher = TestUserPasswordHasherFactory.Criar();
        var (usuario, dto) = TestUsuarioFactory.Criar(passwordHasher);
        await _dbHelper.InsertAsync(usuario);
        var handler = CriarSessaoHandler(passwordHasher);

        var response = await handler.IniciarSessaoAsync(new IniciarSessaoRequest
        {
            Email = dto.Email,
            Senha = dto.Senha
        });

        var sessao = await _dbHelper.SingleAsync<Sessao>(p => p.IdUsuario == usuario.Id);
        var token = await _dbHelper.SingleAsync<RefreshToken>(p => p.IdSessao == sessao.Id);
        AssertAcessoCriadoComSucesso(response, usuario, token);
    }

    [Fact]
    public async Task Renova_acesso_para_refresh_token_valido()
    {
        var passwordHasher = TestUserPasswordHasherFactory.Criar();
        var (usuario, _) = TestUsuarioFactory.Criar(passwordHasher);
        var sessao = TestSessaoFactory.CriarAtiva(usuario);
        var tokenOriginal = TestRefreshTokenFactory.CriarAtivo(sessao);
        await _dbHelper.InsertAsync(usuario, sessao, tokenOriginal);
        var handler = CriarSessaoHandler();

        var response = await handler.RenovarAcessoAsync(new RenovarAcessoRequest
        {
            RefreshToken = tokenOriginal.Token
        });

        var tokens = await _dbHelper.AllAsync<RefreshToken>(p => p.IdSessao == sessao.Id);
        var tokenAtivo = tokens.Should().ContainSingle(p => p.Situacao == SituacaoRefreshToken.Valido).Subject;
        var tokenSubstituido = tokens.Should().ContainSingle(p => p.Id == tokenOriginal.Id).Subject;

        tokenSubstituido.Situacao.Should().Be(SituacaoRefreshToken.Substituido);
        AssertAcessoCriadoComSucesso(response, usuario, tokenAtivo);
    }

    [Fact]
    public async Task Considera_sessao_invalida_caso_token_nao_exista()
    {
        var passwordHasher = TestUserPasswordHasherFactory.Criar();
        var (usuario, _) = TestUsuarioFactory.Criar(passwordHasher);
        var sessao = TestSessaoFactory.CriarAtiva(usuario);
        await _dbHelper.InsertAsync(usuario, sessao);
        var handler = CriarSessaoHandler();

        var response = await handler.RenovarAcessoAsync(new RenovarAcessoRequest
        {
            RefreshToken = "TOKEN"
        });

        var sessaoNoBanco = await _dbHelper.SingleAsync<Sessao>(p => p.Id == sessao.Id);

        sessaoNoBanco.Situacao.Should().Be(SituacaoSessao.Valida);
        AssertResponseAcessoInvalido(response, MotivoFalhaSessaoResponse.SessaoInvalida);
    }

    [Fact]
    public async Task Invalida_sessao_expirada()
    {
        var passwordHasher = TestUserPasswordHasherFactory.Criar();
        var (usuario, _) = TestUsuarioFactory.Criar(passwordHasher);
        var sessao = TestSessaoFactory.CriarAtiva(usuario, TimeSpan.FromMilliseconds(-1));
        var tokenOriginal = TestRefreshTokenFactory.CriarAtivo(sessao);
        await _dbHelper.InsertAsync(usuario, sessao, tokenOriginal);
        var handler = CriarSessaoHandler();

        var response = await handler.RenovarAcessoAsync(new RenovarAcessoRequest
        {
            RefreshToken = tokenOriginal.Token
        });

        var tokens = await _dbHelper.AllAsync<RefreshToken>(p => p.IdSessao == sessao.Id);
        var sessaoNoBanco = await _dbHelper.SingleAsync<Sessao>(p => p.Id == sessao.Id);
        var tokenNoBanco = tokens.Should().ContainSingle(p => p.Id == tokenOriginal.Id).Subject;

        sessaoNoBanco.Situacao.Should().Be(SituacaoSessao.Expirada);
        tokenNoBanco.Situacao.Should().Be(SituacaoRefreshToken.Invalido);
        AssertResponseAcessoInvalido(response, MotivoFalhaSessaoResponse.SessaoInvalida);
    }

    [Fact]
    public async Task Invalida_sessao_se_o_refresh_token_ja_foi_utilizado()
    {
        var passwordHasher = TestUserPasswordHasherFactory.Criar();
        var (usuario, _) = TestUsuarioFactory.Criar(passwordHasher);
        var sessao = TestSessaoFactory.CriarAtiva(usuario);
        var tokenOriginalSubstituido = TestRefreshTokenFactory.CriarInvalido(sessao);
        var tokenOriginalAtivo = TestRefreshTokenFactory.CriarAtivo(sessao);
        await _dbHelper.InsertAsync(usuario, sessao, tokenOriginalSubstituido, tokenOriginalAtivo);
        var handler = CriarSessaoHandler();

        var response = await handler.RenovarAcessoAsync(new RenovarAcessoRequest
        {
            RefreshToken = tokenOriginalSubstituido.Token
        });

        var tokensNoBanco = await _dbHelper.AllAsync<RefreshToken>(p => p.IdSessao == sessao.Id);
        var sessaoNoBanco = await _dbHelper.SingleAsync<Sessao>(p => p.Id == sessao.Id);
        var tokenSubstituido = tokensNoBanco.Should().ContainSingle(p => p.Id == tokenOriginalSubstituido.Id).Subject;
        var tokenRevogado = tokensNoBanco.Should().ContainSingle(p => p.Id == tokenOriginalAtivo.Id).Subject;

        sessaoNoBanco.Situacao.Should().Be(SituacaoSessao.Revogada);
        tokenSubstituido.Situacao.Should().Be(SituacaoRefreshToken.Invalido);
        tokenRevogado.Situacao.Should().Be(SituacaoRefreshToken.Invalido);
        AssertResponseAcessoInvalido(response, MotivoFalhaSessaoResponse.SessaoInvalida);
    }


    private static void AssertAcessoCriadoComSucesso(SessaoResponse response, Usuario usuario, RefreshToken token)
    {
        var usuarioViewEsperado = SessaoUsuarioViewMapper.Map(usuario);

        response.Usuario.Should().BeEquivalentTo(usuarioViewEsperado);
        response.AccessToken?.TimeToLive.Should().Be(TimeSpan.FromSeconds(DuracaoAccessTokenSegundos));

        var accessJwt = JwtHelper.ReadToken(response.AccessToken?.Token ?? "");
        JwtHelper.ReadClaim<Guid>(accessJwt, "sub").Should().Be(usuario.Id);
        JwtHelper.ReadClaim<Guid>(accessJwt, "tid").Should().Be(usuario.IdTenant);
        JwtHelper.ReadClaim<string>(accessJwt, "name").Should().Be(usuario.Nome);
        JwtHelper.ReadClaim<string>(accessJwt, "email").Should().Be(usuario.Email);

        response.RefreshToken?.Token.Should().Be(token.Token);
    }

    private async Task AssertCredenciaisInvalidasAsync(SessaoResponse response)
    {
        var sessoes = await _dbHelper.AllAsync<Sessao>();
        var tokens = await _dbHelper.AllAsync<RefreshToken>();

        sessoes.Should().BeEmpty();
        tokens.Should().BeEmpty();

        AssertResponseAcessoInvalido(response, MotivoFalhaSessaoResponse.CredenciaisInvalidas);
    }

    private static void AssertResponseAcessoInvalido(SessaoResponse response, MotivoFalhaSessaoResponse motivoFalha)
    {
        response.MotivoFalha.Should().Be(motivoFalha);
        response.Usuario.Should().BeNull();
        response.AccessToken.Should().BeNull();
        response.RefreshToken.Should().BeNull();
    }

    private SessaoHandler CriarSessaoHandler(IUserPasswordHasher<Usuario>? passwordHasher = null)
    {
        passwordHasher ??= TestUserPasswordHasherFactory.Criar();

        var defaultDbContext = _dbHelper.CreateDbContext();
        var unitOfWork = new DefaultUnitOfWork<UsuariosDbContext>(defaultDbContext);
        var usuarioRepository = new UsuarioRepository(defaultDbContext);
        var sessaoRepository = new SessaoRepository(defaultDbContext);
        var refreshTokenRepository = new RefreshTokenRepository(defaultDbContext);
        var peerJwtBuilder = new PeerJwtBuilder(StubOptionsSnapshot.Create(new PeerJwtBuilderOptions
        {
            SigningKey = "DB27DA9CFDD94966BFEE08CFA68D43AD"
        }));
        var geradorRefreshToken = TestHashedRefreshTokenGeneratorFactory.Criar();
        var sessaoOptions = TestSessaoOptionsFactory.Criar(
            duracaoSessao: TimeSpan.FromSeconds(DuracaoSessaoSegundos),
            duracaoAccessToken: TimeSpan.FromSeconds(DuracaoAccessTokenSegundos));

        var handler = new SessaoHandler(
            unitOfWork,
            usuarioRepository,
            sessaoRepository,
            refreshTokenRepository,
            passwordHasher,
            peerJwtBuilder,
            geradorRefreshToken,
            sessaoOptions,
            new DefaultNotificationBag());
        return handler;
    }
}
