using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.Domain.Usuarios.Services;

public sealed record ResultadoAutenticacao(bool Sucesso, Usuario? Usuario);
