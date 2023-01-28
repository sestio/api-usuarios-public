using Sestio.Usuarios.App.Services.Sessoes.Responses;

namespace Sestio.Usuarios.Api.Rest.Sessoes;

[Serializable]
public sealed class SessaoView
{
    public required SessaoUsuarioResponse Usuario { get; set; }
    public required AccessTokenView AccessToken { get; set; }
}
