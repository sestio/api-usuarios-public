using Sestio.Usuarios.Domain.Sessoes.Entities;

namespace Sestio.Usuarios.Domain.Hashing;

public interface IGeradorRefreshToken
{
    string Gerar(Sessao sessao);
}
