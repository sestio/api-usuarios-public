using Sestio.Usuarios.Domain.Sessoes.Entities;
using Sestio.Usuarios.Domain.Sessoes.Values;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.Domain.Sessoes.Services;

public sealed record DadosAcesso(
    Usuario Usuario,
    AccessToken AccessToken,
    RefreshToken RefreshToken);
