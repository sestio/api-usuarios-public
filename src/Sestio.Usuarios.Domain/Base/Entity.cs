using Sestio.Commons.Domain;

namespace Sestio.Usuarios.Domain.Base;

public class Entity : IIdentifiable
{
    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }
}
