using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Sestio.Usuarios.Infra.EntityFramework;

namespace Sestio.Usuarios.Startup.Migrations;

public class MigrationDbContextFactory :
    IDesignTimeDbContextFactory<UsuariosDbContext>
{
    UsuariosDbContext IDesignTimeDbContextFactory<UsuariosDbContext>.CreateDbContext(string[] args)
    {
        var options = BuildDbContextOptions<UsuariosDbContext>();
        return new UsuariosDbContext(options);
    }

    private static DbContextOptions<TDbContext> BuildDbContextOptions<TDbContext>()
        where TDbContext : DbContext
    {
        var configuration = BuildConfiguration();
        var connectionString = configuration.GetValue<string>("Database:ConnectionString");

        var options = new DbContextOptionsBuilder<TDbContext>()
            .UseNpgsql(connectionString,
                       p => p.MigrationsAssembly(typeof(MigrationDbContextFactory).Assembly.GetName().Name))
            .Options;
        return options;
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        configurationBuilder.AddJsonFile("appsettings.json", optional: true);
        configurationBuilder.AddJsonFile($"appsettings.{environment}.json", optional: true);
        configurationBuilder.AddJsonFile("appsettings.local.json", optional: true);

        return configurationBuilder.Build();
    }
}
