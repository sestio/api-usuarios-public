using Sestio.Commons.Startup.Extensions.Configuration;
using Sestio.Commons.Startup.Extensions.HealthChecks;
using Sestio.Commons.Startup.Extensions.JsonWebTokens;
using Sestio.Commons.Startup.Extensions.Mvc;
using Sestio.Commons.Validation.Core;
using Sestio.Commons.Validation.Services;
using Sestio.Usuarios.Domain.Sessoes.Services;
using Sestio.Usuarios.Startup;
using Sestio.Usuarios.Startup.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ReconfigureSources();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<INotificationBag, DefaultNotificationBag>();

builder.Services.AddHealthChecks<ReadinessCheck, AlwaysLiveHealthCheck>();
builder.Services.AddPeerJwtBuilder(options =>
{
    builder.Configuration.GetSection("PeerJwt").Bind(options);
});

var sessaoOptions = new SessaoOptions(
    DuracaoSessao: TimeSpan.FromMinutes(builder.Configuration.GetValue<double>("Sessao:MinutosDuracaoSessao")),
    DuracaoAccessToken: TimeSpan.FromMinutes(builder.Configuration.GetValue<double>("Sessao:MinutosDuracaoAccessToken")));
builder.Services.AddSingleton(sessaoOptions);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplication();
builder.Services.AddDomain();
builder.Services.AddControllers().ConfigureJsonOptions();


var app = builder.Build();

app.AddExceptionHandling();
app.UseHealthChecks();
app.UseAuthorization();
app.MapControllers();

app.Run();
