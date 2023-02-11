using FluentAssertions;
using Sestio.Usuarios.Domain.Usuarios.Services;

namespace Sestio.Usuarios.UnitTests.Specs.Domain.Usuarios;

public class ValidadorEmailTests
{
    [Theory]
    [InlineData("email@example.com")]
    [InlineData("email@example.com.br")]
    [InlineData("e.mail@example.com.br")]
    [InlineData("e-mail@example-br.com")]
    public void Aceita_emails_validos(string email)
    {
        var result = ValidadorEmail.IsValid(email);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("email")]
    [InlineData("email@example")]
    [InlineData("example.com")]
    [InlineData("@example.com")]
    [InlineData(".@example.com")]
    [InlineData("email.@example.com")]
    [InlineData(".email@example.com")]
    [InlineData("email@example..com")]
    [InlineData("email@.example.com")]
    [InlineData("email@example.com.")]
    public void Rejeita_emails_invalidos(string email)
    {
        var result = ValidadorEmail.IsValid(email);

        result.Should().BeFalse();
    }
}
