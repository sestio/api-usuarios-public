using Sestio.Commons.Validation.Core;
using Sestio.Commons.Validation.Services;
using Sestio.Usuarios.App.Services.Usuarios.Requests;
using Sestio.Usuarios.Domain.Usuarios.Entities;
using Sestio.Usuarios.Domain.Usuarios.Services;

namespace Sestio.Usuarios.App.Handlers.Usuarios.Validadores;

internal class ValidadorRequests
{
    private readonly IUsuarioRepository _usuarioRepository;

    internal ValidadorRequests(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    internal async Task ValidarAsync(CriarUsuarioRequest request)
    {
        var notifications = new DefaultNotificationBag();

        ValidarTenant(notifications, request);
        ValidarNome(notifications, request);
        await ValidarEmailAsync(notifications, request);
        ValidarSenha(notifications, request);

        if (notifications.HasErrors)
            throw new NotificationException(notifications.All());
    }

    private static void ValidarTenant(INotificationBag notifications, CriarUsuarioRequest request)
    {
        if (request.IdTenant == Guid.Empty)
            notifications.AddError("REQUIRED_TENANT", "Tenant não informado");
    }

    private static void ValidarNome(INotificationBag notifications, CriarUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            notifications.AddError("REQUIRED_NOME", "Nome não informado");
    }

    private async Task ValidarEmailAsync(DefaultNotificationBag notifications, CriarUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            notifications.AddError("REQUIRED_EMAIL", "E-mail não informado");
        else if (!ValidadorEmail.IsValid(request.Email))
            notifications.AddError("INVALID_EMAIL", "O e-mail informado não é válido");
        else if (await _usuarioRepository.ExistePorEmailAsync(request.Email))
            notifications.AddError("DUPLICATED_EMAIL", $"O e-mail '{request.Email}' já foi cadastrado");
    }

    private static void ValidarSenha(INotificationBag notifications, CriarUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Senha))
            notifications.AddError("REQUIRED_SENHA", "Senha não informada");
    }
}
