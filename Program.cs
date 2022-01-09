using FriendlyBackup.BackupManagement;
using FriendlyBackup.BackupManagement.Testing;
using FriendlyBackup.Configuration;
using FriendlyBackup.Repositories;
using FriendlyBackup.Repositories.Implementation;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IBackupRepository, FileBackupRepository>();
builder.Services.AddSingleton<IBackupConnector, FakeBackupConnector>();
builder.Services.AddSingleton<BackupSpecs>();

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var configuration = configBuilder.Build();

builder.Services.AddOptions();
builder.Services.Configure<LocalPathsConfig>(configuration.GetSection("LocalPathsConfig"));

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
