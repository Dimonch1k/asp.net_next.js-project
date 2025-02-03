using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend_c_.Controllers;
using backend_c_.DTO.User;
using backend_c_.Entity;
using backend_c_.Enums;
using backend_c_.Exceptions;
using backend_c_.Utilities;
using backend_c_.Utils;
using Microsoft.IdentityModel.Tokens;

namespace backend_c_.Service.Impl;

public class AuthService : IAuthService
{
  private readonly AppDbContext _dbContext;
  private readonly Lazy<IUserService> _userService;
  private readonly ILogger<AuthService> _logger;
  private readonly IConfiguration _configuration;

  public AuthService( AppDbContext dbContext, Lazy<IUserService> userService, ILogger<AuthService> logger, IConfiguration configuration )
  {
    _dbContext = dbContext;
    _userService = userService;
    _logger = logger;
    _configuration = configuration;
  }

  public bool Authorize( AuthDto authDto )
  {
    try
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      string? secret = _configuration["Jwt:Secret"];

      CheckIfSecretIsMissing( secret );

      byte[] key = Encoding.ASCII.GetBytes( _configuration["Jwt:Secret"] );

      tokenHandler.ValidateToken( authDto.Token, new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey( key ),
        ValidateIssuer = false,
        ValidateAudience = false
      }, out _ );

      return true;
    }
    catch ( SecurityTokenException )
    {
      _logger.LogInformation( "Invalid token" );

      throw new ServerException( "Invalid token", ExceptionStatusCode.InvalidToken );
    }
    catch ( Exception ex )
    {
      _logger.LogInformation( $"Authorization failed: {ex.Message}" );

      throw new ServerException( "Authorization failed", ExceptionStatusCode.InternalServerError );
    }
  }

  public string? Login( LoginDto loginDto )
  {
    User? user = _dbContext.Users.FirstOrDefault( u => u.Username == loginDto.Username );

    _userService.Value.EnsureUserIsNotNull( user );

    if ( !HashingHelper.VerifyPassword( loginDto.Password, user.PasswordHash ) )
    {
      _logger.LogInformation( "Invalid username or password" );

      throw new ServerException( "Invalid username or password", ExceptionStatusCode.BadRequest );
    }

    return GenerateJwtToken( user );
  }

  private ClaimsIdentity GenerateClaims( User user )
  {
    ClaimsIdentity claimsIdentity = new ClaimsIdentity();

    claimsIdentity.AddClaim( new Claim( "id", user.Id.ToString() ) );

    return claimsIdentity;
  }

  private string GenerateJwtToken( User user )
  {
    string? secret = _configuration["Jwt:Secret"];

    CheckIfSecretIsMissing( secret );

    var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( secret ) );
    var credentials = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );

    var claimsList = new[]
    {
       new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() ),
       new Claim(JwtRegisteredClaimNames.Sub,  user.Id.ToString())
    };

    var token = new JwtSecurityToken(
      expires: DateTime.Now.AddHours( 24 ),
      signingCredentials: credentials
    );
    token.Payload.AddClaim( new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() ) );
    token.Payload.AddClaim( new Claim( JwtRegisteredClaimNames.Sub, user.Id.ToString() ) );

    return new JwtSecurityTokenHandler().WriteToken( token );
  }

  private void CheckIfSecretIsMissing( string? secret )
  {
    if ( string.IsNullOrEmpty( secret ) )
    {
      _logger.LogInformation( "JWT secret is missing" );

      throw new ServerException( "JWT secret is missing", ExceptionStatusCode.MissingJwtSecret );
    }
  }
}
