using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sestio.Commons.Domain;
using Sestio.Commons.Infra.EntityFramework;
using Sestio.Commons.Infra.EntityFramework.Extensions;
using Sestio.Usuarios.App.Handlers.Usuarios;
using Sestio.Usuarios.App.Services.Usuarios.Services;
using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Usuarios;
using Sestio.Usuarios.Infra.Domain.Hashing;
using Sestio.Usuarios.Infra.EntityFramework;
using Sestio.Usuarios.Infra.Repositories.Usuarios;

namespace Sestio.Usuarios.Startup.Extensions;

public static class ServiceContainerExtensions
{
    private sealed class DatabaseOptions
    {
        public required string ConnectionString { get; set; }
        public required bool EnableDetailedErrors { get; set; }
        public required bool EnableSensitiveDataLogging { get; set; }
    }

    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UsuarioDbContext>(options =>
        {
            var databaseOptions = configuration.GetSection("Database").Get<DatabaseOptions>()!;
            options.UseNpgsql(databaseOptions.ConnectionString);
            options.Configure(options =>
            {
                options.EnableDetailedErrors = databaseOptions.EnableDetailedErrors;
                options.EnableSensitiveDataLogging = databaseOptions.EnableSensitiveDataLogging;
            });
        });
        services.AddScoped<IUnitOfWork, DefaultUnitOfWork<UsuarioDbContext>>();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
    }

    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserPasswordHasher<Usuario>, UserPasswordHasher<Usuario>>();
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

        services.AddScoped<IUsuarioService, UsuarioHandler>();
    }
}