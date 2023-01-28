namespace Sestio.Usuarios.App.Services.Sessoes.Responses;

public sealed record SessaoResponse(
    bool Sucesso,
    MotivoFalhaSessaoResponse MotivoFalha,
    SessaoUsuarioResponse? Usuario,
    AccessTokenResponse? AccessToken,
    RefreshTokenResponse? RefreshToken);
