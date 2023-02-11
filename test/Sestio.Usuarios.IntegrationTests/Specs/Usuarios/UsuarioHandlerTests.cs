using FluentAssertions;
using Sestio.Commons.Infra.EntityFramework;
using Sestio.Commons.Validation.Services;
using Sestio.Usuarios.App.Handlers.Usuarios;
using Sestio.Usuarios.App.Services.Usuarios.Requests;
using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Usuarios.Entities;
using Sestio.Usuarios.Infra.EntityFramework;
using Sestio.Usuarios.Infra.Repositories.Usuarios;
using Sestio.Usuarios.IntegrationTests.Base.Helpers;
using Sestio.Usuarios.TestLib.Factories;
using Sestio.Usuarios.TestLib.Stubs;

namespace Sestio.Usuarios.IntegrationTests.Specs.Usuarios;

public class UsuarioHandlerTests
{
    private readonly DbHelper _dbHelper;

    public UsuarioHandlerTests()
    {
        _dbHelper = new DbHelper(nameof(UsuarioHandlerTests));
        _dbHelper.CreateDbContext().Database.EnsureCreated();
    }

    [Fact]
    public async Task Rejeita_cadastro_de_usuario_sem_tenant_informado()
    {
        var request = new CriarUsuarioRequest
        {
            IdTenant = Guid.Empty,
            Nome = "User",
            Email = "email@example.com",
            Senha = "password"
        };
        var sut = CriarUsuarioHandler();

        var act = () => sut.CriarUsuarioAsync(request);

        var ex = (await act.Should().ThrowAsync<NotificationException>()).Subject.Single();
        var notification = ex.Notifications.Should().ContainSingle().Subject;

        notification.Code.Should().Be("REQUIRED_TENANT");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Rejeita_cadastro_de_usuario_sem_nome_informado(string nome)
    {
        var request = TestUsuarioRequestFactory.CriarUsuario(nome: nome);
        var sut = CriarUsuarioHandler();

        var act = () => sut.CriarUsuarioAsync(request);

        var ex = (await act.Should().ThrowAsync<NotificationException>()).Subject.Single();
        var notification = ex.Notifications.Should().ContainSingle().Subject;

        notification.Code.Should().Be("REQUIRED_NOME");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Rejeita_cadastro_de_usuario_sem_email_informado(string email)
    {
        var request = TestUsuarioRequestFactory.CriarUsuario(email: email);
        var sut = CriarUsuarioHandler();

        var act = () => sut.CriarUsuarioAsync(request);

        var ex = (await act.Should().ThrowAsync<NotificationException>()).Subject.Single();
        var notification = ex.Notifications.Should().ContainSingle().Subject;

        notification.Code.Should().Be("REQUIRED_EMAIL");
    }

    [Fact]
    public async Task Rejeita_cadastro_de_usuario_com_email_invalido()
    {
        var request = TestUsuarioRequestFactory.CriarUsuario(email: "invalid@email");
        var sut = CriarUsuarioHandler();

        var act = () => sut.CriarUsuarioAsync(request);

        var ex = (await act.Should().ThrowAsync<NotificationException>()).Subject.Single();
        var notification = ex.Notifications.Should().ContainSingle().Subject;

        notification.Code.Should().Be("INVALID_EMAIL");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Rejeita_cadastro_de_usuario_sem_senha_informada(string senha)
    {
        var request = TestUsuarioRequestFactory.CriarUsuario(senha: senha);
        var sut = CriarUsuarioHandler();

        var act = () => sut.CriarUsuarioAsync(request);

        var ex = (await act.Should().ThrowAsync<NotificationException>()).Subject.Single();
        var notification = ex.Notifications.Should().ContainSingle().Subject;

        notification.Code.Should().Be("REQUIRED_SENHA");
    }

    [Fact]
    public async Task Rejeita_cadastro_de_usuario_com_email_ja_utilizado()
    {
        var (usuarioExistente, _) = TestUsuarioFactory.Criar();
        var request = TestUsuarioRequestFactory.CriarUsuario(email: usuarioExistente.Email);
        await _dbHelper.InsertAsync(usuarioExistente);
        var sut = CriarUsuarioHandler();
        var act = () => sut.CriarUsuarioAsync(request);

        var ex = (await act.Should().ThrowAsync<NotificationException>()).Subject.Single();
        var notification = ex.Notifications.Should().ContainSingle().Subject;

        notification.Code.Should().Be("DUPLICATED_EMAIL");
    }

    [Fact]
    public async Task Normaliza_email_do_usuario_durante_o_cadastro()
    {
        var request = TestUsuarioRequestFactory.CriarUsuario(email: "email@example.com");
        var sut = CriarUsuarioHandler();

        await sut.CriarUsuarioAsync(request);

        var usuario = await _dbHelper.SingleAsync<Usuario>();

        usuario.Email.Should().Be("EMAIL@EXAMPLE.COM");
    }

    [Fact]
    public async Task Criptografa_senha_do_usuario_durante_o_cadastro()
    {
        var request = TestUsuarioRequestFactory.CriarUsuario(email: "EMAIL@EXAMPLE.COM");
        var sut = CriarUsuarioHandler(new StubUserPasswordHasher("HASH"));

        await sut.CriarUsuarioAsync(request);

        var usuario = await _dbHelper.SingleAsync<Usuario>();
        usuario.Senha.Should().BeEquivalentTo(new HashedPassword("HASH"));
    }

    private UsuarioHandler CriarUsuarioHandler(IUserPasswordHasher<Usuario>? passwordHasher = null)
    {
        passwordHasher ??= TestUserPasswordHasherFactory.Criar();

        var defaultDbContext = _dbHelper.CreateDbContext();
        var unitOfWork = new DefaultUnitOfWork<UsuariosDbContext>(defaultDbContext);
        var usuarioRepository = new UsuarioRepository(defaultDbContext);

        var handler = new UsuarioHandler(unitOfWork, usuarioRepository, passwordHasher);
        return handler;
    }
}
