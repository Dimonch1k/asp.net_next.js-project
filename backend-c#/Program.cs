using backend_c_;
using backend_c_.Validators.User;
using dotenv.net;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

// Load environment variables from .env
DotEnv.Load();

// Read the connection string from environment variables
string? postgresConnection = Environment.GetEnvironmentVariable( "PostgreSqlConnection" );

// Configure the ConnectionStrings in Configuration
builder.Configuration["ConnectionStrings:PostgreSqlConnection"] = postgresConnection;

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext for PostgreSQL
builder.Services.AddDbContext<AppDbContext>( options =>
    options.UseNpgsql( builder.Configuration.GetConnectionString( "PostgreSqlConnection" ) ) );

// Configure FluentValidation validators
builder.Services.AddFluentValidation( fv =>
{
  fv.RegisterValidatorsFromAssemblyContaining<LoginDtoValidator>();
} );

// Configure logging
builder.Logging
    .ClearProviders()
    .AddConsole();


WebApplication app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
app.UseAuthorization();
app.MapControllers();
app.Run();
