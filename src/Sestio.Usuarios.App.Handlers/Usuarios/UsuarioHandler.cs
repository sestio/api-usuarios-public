using Sestio.Commons.Domain;
using Sestio.Usuarios.App.Handlers.Usuarios.Mappers;
using Sestio.Usuarios.App.Services.Usuarios.Requests;
using Sestio.Usuarios.App.Services.Usuarios.Responses;
using Sestio.Usuarios.App.Services.Usuarios.Services;
using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.App.Handlers.Usuarios;

public class UsuarioHandler : IUsuarioHandler
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

    public async Task<UsuarioResponse> CriarUsuarioAsync(CriarUsuarioRequest request)
    {
        // TODO: Adicionar validações quando deixar de ser apenas para desenvolvimento
        var dto = CriarUsuarioDtoMapper.Map(request);
        var usuario = _usuarioFactory.Criar(dto);

        _usuarioRepository.Add(usuario);
        await _unitOfWork.SaveChangesAsync();

        var view = MapToResponse(usuario);
        return view;
    }

    public async Task<List<UsuarioResponse>> ListarAsync()
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
        var views = usuarios.Select(MapToResponse).ToList();
        return views;
    }

    private static UsuarioResponse MapToResponse(Usuario user)
    {
        return new UsuarioResponse
        {
            Id = user.Id,
            IdTenant = user.IdTenant,
            Nome = user.Nome,
            Email = user.Email
        };
    }
}
