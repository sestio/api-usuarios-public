using Sestio.Commons.Domain;
using Sestio.Usuarios.App.Handlers.Usuarios.Mappers;
using Sestio.Usuarios.App.Handlers.Usuarios.Validadores;
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
    private readonly ValidadorRequests _validadorRequests;

    public UsuarioHandler(
        IUnitOfWork unitOfWork,
        IUsuarioRepository usuarioRepository,
        IUserPasswordHasher<Usuario> passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _usuarioRepository = usuarioRepository;
        _usuarioFactory = new UsuarioFactory(passwordHasher);
        _validadorRequests = new ValidadorRequests(usuarioRepository);
    }

    public async Task<UsuarioResponse> CriarUsuarioAsync(CriarUsuarioRequest request)
    {
        await _validadorRequests.ValidarAsync(request);

        var dto = CriarUsuarioDtoMapper.Map(request);
        var usuario = _usuarioFactory.Criar(dto);

        _usuarioRepository.Add(usuario);
        await _unitOfWork.SaveChangesAsync();

        var view = UsuarioResponseMapper.Map(usuario);
        return view;
    }

    public async Task<List<UsuarioResponse>> ListarAsync()
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
        var views = usuarios.Select(UsuarioResponseMapper.Map).ToList();
        return views;
    }
}
