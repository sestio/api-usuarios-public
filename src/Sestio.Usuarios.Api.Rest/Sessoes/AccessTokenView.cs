namespace Sestio.Usuarios.Api.Rest.Sessoes;

[Serializable]
public sealed class AccessTokenView
{
    public required string Token { get; init; }
    public required long Ttl { get; init; }
}
