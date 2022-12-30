using Sestio.Commons.Domain;
using Sestio.Usuarios.App.Handlers.Usuarios.Mappers;
using Sestio.Usuarios.App.Services.Usuarios.Commands;
using Sestio.Usuarios.App.Services.Usuarios.Services;
using Sestio.Usuarios.App.Services.Usuarios.Views;
using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Usuarios;

namespace Sestio.Usuarios.App.Handlers.Usuarios;

public class UsuarioHandler : IUsuarioService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly UsuarioFactory _usuarioFactory;

    public UsuarioHandler(
        IUnitOfWork unitOfWork,
        IUsuarioRepository usuarioRepository,
        IUserPasswordHasher<Usuario> passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _usuarioRepository = usuarioRepository;
        _usuarioFactory = new UsuarioFactory(passwordHasher);
    }

    public async Task<UsuarioView> CriarUsuarioAsync(CriarUsuarioCommand command)
    {
        // TODO: Adicionar validações quando deixar de ser apenas para desenvolvimento
        var dto = CriarUsuarioDtoMapper.Map(command);
        var usuario = _usuarioFactory.Criar(dto);

        await _usuarioRepository.AddAsync(usuario);
        await _unitOfWork.SaveChangesAsync();

        var view = MapToView(usuario);
        return view;
    }

    public async Task<List<UsuarioView>> ListarAsync()
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
        var views = usuarios.Select(MapToView).ToList();
        return views;
    }

    private static UsuarioView MapToView(Usuario user)
    {
        return new UsuarioView
        {
            Id = user.Id,
            IdTenant = user.IdTenant,
            Nome = user.Nome,
            Email = user.Email
        };
    }
}
