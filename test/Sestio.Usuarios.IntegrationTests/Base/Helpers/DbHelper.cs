﻿using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sestio.Usuarios.Infra.EntityFramework;

namespace Sestio.Usuarios.IntegrationTests.Base.Helpers;

public sealed class DbHelper
{
    private readonly DatabaseConnectionSettings _connectionSettings;

    public DbHelper(string dbNamePrefix)
    {
        _connectionSettings = LoadConnectionSettings().WithRandomDbName(dbNamePrefix);
    }

    public UsuariosDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<UsuariosDbContext>()
            .UseNpgsql(_connectionSettings.ConnectionString)
            .UseLoggerFactory(LoggerFactory.Create(p => p.AddConsole()))
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        var context = new UsuariosDbContext(options);
        return context;
    }

    private static DatabaseConnectionSettings LoadConnectionSettings()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("dbsettings.json")
            .AddJsonFile("dbsettings.local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return DatabaseConnectionSettings.FromConfiguration(configuration);
    }

    public async Task InsertAsync<T>(T entity)
        where T : class
    {
        var context = CreateDbContext();
        context.Set<T>().Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task InsertAsync<T1, T2>(T1 entity1, T2 entity2)
        where T1 : class
        where T2 : class
    {
        var context = CreateDbContext();
        context.Set<T1>().Add(entity1);
        context.Set<T2>().Add(entity2);
        await context.SaveChangesAsync();
    }

    public async Task InsertAsync<T1, T2, T3>(T1 entity1, T2 entity2, T3 entity3)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        var context = CreateDbContext();
        context.Set<T1>().Add(entity1);
        context.Set<T2>().Add(entity2);
        context.Set<T3>().Add(entity3);
        await context.SaveChangesAsync();
    }

    public async Task InsertAsync<T1, T2, T3, T4>(T1 entity1, T2 entity2, T3 entity3, T4 entity4)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        var context = CreateDbContext();
        context.Set<T1>().Add(entity1);
        context.Set<T2>().Add(entity2);
        context.Set<T3>().Add(entity3);
        context.Set<T4>().Add(entity4);
        await context.SaveChangesAsync();
    }

    public async Task<T> SingleAsync<T>(Expression<Func<T, bool>> filter)
        where T : class
    {
        var all = await AllAsync(filter);
        all.Should().ContainSingle();
        return all.Single();
    }

    public async Task<List<T>> AllAsync<T>(Expression<Func<T, bool>>? filter = null)
        where T : class
    {
        var context = CreateDbContext();
        var query = context.Set<T>().AsQueryable();

        if (filter != null)
            query = query.Where(filter);

        var result = await query.ToListAsync();
        return result;
    }
}
