using Autofac.Core;
using backend_c_;
using backend_c_.Enums;
using backend_c_.Exceptions;
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

DotEnv.Load();

string? postgresConnection = Environment.GetEnvironmentVariable( "PostgreSqlConnection" );
string? jwtSecret = Environment.GetEnvironmentVariable( "JwtSecret" );
string? PATH_BASE_DIR = Environment.GetEnvironmentVariable( "PATH_BASE_DIRECTORY_WINDOWS" );

if ( string.IsNullOrEmpty( postgresConnection ) )
{
  throw new ServerException( "PostgreSqlConnection is not defined in the environment variables.", ExceptionStatusCode.InternalServerError );
}

if ( string.IsNullOrEmpty( jwtSecret ) )
{
  throw new ServerException( "JwtSecret is not defined in the environment variables.", ExceptionStatusCode.InternalServerError );
}

if ( string.IsNullOrEmpty( PATH_BASE_DIR ) )
{
  throw new ServerException( "PATH_BASE_DIRECTORY_WINDOWS is not defined in the environment variables.", ExceptionStatusCode.InternalServerError );
}

builder.Configuration["Jwt:Secret"] = jwtSecret;

builder.Configuration["ConnectionStrings:PostgreSqlConnection"] = postgresConnection;

builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFileService, MediaFileService>();
builder.Services.AddScoped<ISharedFileService, SharedFileService>();
builder.Services.AddScoped<IAccessLogService, AccessLogService>();
builder.Services.AddScoped<IVersionService, VersionService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped( typeof( Lazy<> ), typeof( LazyDependency<> ) );

builder.Services.AddDbContext<AppDbContext>( options =>
    options.UseNpgsql( postgresConnection ) );

builder.Services.AddFluentValidationAutoValidation()
  .AddFluentValidationClientsideAdapters()
  .AddValidatorsFromAssemblyContaining<LoginDtoValidator>();


builder.Logging
    .ClearProviders()
    .AddConsole();

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

WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
