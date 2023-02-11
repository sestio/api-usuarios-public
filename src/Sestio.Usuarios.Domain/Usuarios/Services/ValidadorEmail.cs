using System.Text.RegularExpressions;

namespace Sestio.Usuarios.Domain.Usuarios.Services;

public static partial class ValidadorEmail
{
    private static readonly Regex EmailRegex = BuildEmailRegex();

    [GeneratedRegex("^[^@.]+.+[^@.]+@[^.]+(\\.[^.]+)+$")]
    private static partial Regex BuildEmailRegex();

    public static bool IsValid(string email)
    {
        return EmailRegex.IsMatch(email);
    }
}
