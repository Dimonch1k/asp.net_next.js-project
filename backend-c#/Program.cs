using backend_c_;
using backend_c_.Service;
using backend_c_.Service.Impl;
using backend_c_.Validators.User;
using dotenv.net;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

// Load environment variables from .env
DotEnv.Load();

// Read the connection string from environment variables
string? postgresConnection = Environment.GetEnvironmentVariable( "PostgreSqlConnection" );
string? jwtSecret = Environment.GetEnvironmentVariable( "JwtSecret" );

// Ensure that necessary environment variables are loaded
if ( string.IsNullOrEmpty( postgresConnection ) )
{
  throw new InvalidOperationException( "PostgreSqlConnection is not defined in the environment variables." );
}

if ( string.IsNullOrEmpty( jwtSecret ) )
{
  throw new InvalidOperationException( "JwtSecret is not defined in the environment variables." );
}

builder.Configuration["Jwt:Secret"] = jwtSecret;

// Configure the ConnectionStrings in Configuration
builder.Configuration["ConnectionStrings:PostgreSqlConnection"] = postgresConnection;

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFileService, MediaFileService>();
builder.Services.AddScoped<ISharedFileService, SharedFileService>();
builder.Services.AddScoped<IAccessLogService, AccessLogService>();
builder.Services.AddScoped<IVersionService, VersionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Configure DbContext for PostgreSQL
builder.Services.AddDbContext<AppDbContext>( options =>
    options.UseNpgsql( postgresConnection ) );

// Configure FluentValidation validators
builder.Services.AddFluentValidation( fv =>
{
  fv.RegisterValidatorsFromAssemblyContaining<LoginDtoValidator>();
} );

// Configure logging
builder.Logging
    .ClearProviders()
    .AddConsole();

// Configure JWT authentication
builder.Services.AddAuthentication( options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
} )
  .AddJwtBearer( options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      IssuerSigningKey = new SymmetricSecurityKey( Encoding.ASCII.GetBytes( jwtSecret ) ),
      ValidateIssuer = false,
      ValidateAudience = false
    };

    options.Events = new JwtBearerEvents
    {
      OnAuthenticationFailed = context =>
      {
        Console.WriteLine( "\n\nAuthentication failed: " + context.Exception.Message );
        return Task.CompletedTask;
      },
      OnTokenValidated = context =>
      {
        Console.WriteLine( "\n\nToken validated successfully" );
        return Task.CompletedTask;
      }
    };
  } );


// Build the application
WebApplication app = builder.Build();

// Configure the middleware pipeline
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

// Run the application
app.Run();
