using backend_c_;
using dotenv.net;
using Microsoft.EntityFrameworkCore;
using VirusTotalMicroService.Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

// Load environment variables from .env
DotEnv.Load();

// Read the connection string from environment variables
string? postgresConnection = Environment.GetEnvironmentVariable( "PostgreSqlConnection" );
string? fileBaseDirectory = Environment.GetEnvironmentVariable( "FILE_BASE_DIRECTORY" );
string? virusTotalApiKey = Environment.GetEnvironmentVariable( "VirusTotalApiKey" );

// Ensure that necessary environment variables are loaded

if ( string.IsNullOrEmpty( postgresConnection ) )
{
  throw new InvalidOperationException( "Missing required PostgreSqlConnection environment variable." );
}

if ( string.IsNullOrEmpty( fileBaseDirectory ) )
{
  throw new InvalidOperationException( "Missing FILE_BASE_DIRECTORY environment variable." );
}

if ( string.IsNullOrEmpty( virusTotalApiKey ) )
{
  throw new InvalidOperationException( "Missing VirusTotalApiKey environment variable." );
}

// Configure DbContext for PostgreSQL
builder.Services.AddDbContext<AppDbContext>( options =>
    options.UseNpgsql( postgresConnection ) );

// Register VirusTotalScannerService as a singleton...
builder.Services.AddSingleton<VirusTotalScannerService>();
// ...and register it as a hosted service using the same instance.
builder.Services.AddHostedService<VirusTotalScannerService>( provider => provider.GetRequiredService<VirusTotalScannerService>() );

builder.Services.AddControllers();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.MapControllers();

app.Run();
