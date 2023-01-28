using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sestio.Commons.Domain;
using Sestio.Commons.Infra.EntityFramework;
using Sestio.Commons.Infra.EntityFramework.Extensions;
using Sestio.Usuarios.App.Handlers.Sessoes;
using Sestio.Usuarios.App.Handlers.Usuarios;
using Sestio.Usuarios.App.Services.Sessoes.Services;
using Sestio.Usuarios.App.Services.Usuarios.Services;
using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Sessoes.Entities;
using Sestio.Usuarios.Domain.Usuarios.Entities;
using Sestio.Usuarios.Infra.Domain.Hashing;
using Sestio.Usuarios.Infra.EntityFramework;
using Sestio.Usuarios.Infra.Repositories.Sessoes;
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
        services.AddDbContext<UsuariosDbContext>(options =>
        {
            var databaseOptions = configuration.GetSection("Database").Get<DatabaseOptions>()!;
            options.UseNpgsql(databaseOptions.ConnectionString);
            options.Configure(options =>
            {
                options.EnableDetailedErrors = databaseOptions.EnableDetailedErrors;
                options.EnableSensitiveDataLogging = databaseOptions.EnableSensitiveDataLogging;
            });
        });
        services.AddScoped<IUnitOfWork, DefaultUnitOfWork<UsuariosDbContext>>();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ISessaoRepository, SessaoRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    }

    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserPasswordHasher<Usuario>, UserPasswordHasher<Usuario>>();
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

        services.AddScoped<IUsuarioHandler, UsuarioHandler>();
        services.AddScoped<ISessaoHandler, SessaoHandler>();
    }

    public static void AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IGeradorRefreshToken, HashedRefreshTokenGenerator>();
    }
}
