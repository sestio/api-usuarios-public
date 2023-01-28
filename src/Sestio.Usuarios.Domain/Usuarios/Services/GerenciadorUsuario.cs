using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.Domain.Usuarios.Services;

public sealed record CredenciaisUsuario(string Email, string Senha);

public sealed class GerenciadorUsuario
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUserPasswordHasher<Usuario> _passwordHasher;

    public GerenciadorUsuario(IUsuarioRepository usuarioRepository,
                              IUserPasswordHasher<Usuario> passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ResultadoAutenticacao> ObterUsuarioAutenticadoAsync(CredenciaisUsuario credenciais)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(credenciais.Email);

        if (usuario == null || !_passwordHasher.IsValid(usuario, credenciais.Senha))
            return new ResultadoAutenticacao(false, null);

        return new ResultadoAutenticacao(true, usuario);
    }
}
