using Sestio.Commons.Startup.Extensions.Configuration;
using Sestio.Commons.Startup.Extensions.HealthChecks;
using Sestio.Commons.Startup.Extensions.Mvc;
using Sestio.Usuarios.Startup;
using Sestio.Usuarios.Startup.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ReconfigureSources();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHealthChecks<ReadinessCheck, AlwaysLiveHealthCheck>();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplication();
builder.Services.AddControllers().ConfigureJsonOptions();


var app = builder.Build();

app.UseHealthChecks();
app.UseAuthorization();
app.MapControllers();

app.Run();
