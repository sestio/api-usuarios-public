using Microsoft.Extensions.Options;

namespace Sestio.Usuarios.IntegrationTests.Base.Stubs;

public static class StubOptionsSnapshot
{
    public static StubOptionsSnapshot<T> Create<T>(T value)
        where T : class
    {
        return new StubOptionsSnapshot<T>(value);
    }
}

public class StubOptionsSnapshot<T> : IOptionsSnapshot<T>
    where T : class
{
    public StubOptionsSnapshot(T value)
    {
        Value = value;
    }

    public T Value { get; }

    public T Get(string? name)
    {
        return Value;
    }
}
