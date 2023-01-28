using Sestio.Usuarios.Domain.Hashing;

namespace Sestio.Usuarios.Domain.Usuarios.Entities;

public sealed record CriarUsuarioDto(
    Guid IdTenant,
    string Nome,
    string Email,
    string Senha);

public sealed class UsuarioFactory
{
    private readonly IUserPasswordHasher<Usuario> _passwordHasher;

    public UsuarioFactory(IUserPasswordHasher<Usuario> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public Usuario Criar(CriarUsuarioDto dto)
    {
        var usuario = new Usuario(dto.IdTenant, dto.Nome, dto.Email);
        var hashedPassword = _passwordHasher.Hash(usuario, dto.Senha);
        usuario.AtualizarSenha(hashedPassword);
        return usuario;
    }
}
