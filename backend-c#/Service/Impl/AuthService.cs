using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend_c_.DTO.User;
using backend_c_.Entity;
using backend_c_.Utils;
using Microsoft.IdentityModel.Tokens;

namespace backend_c_.Service.Impl;

public class AuthService : IAuthService
{
  private readonly IConfiguration _configuration;
  private readonly AppDbContext _context;

  public AuthService( AppDbContext context, IConfiguration configuration )
  {
    _configuration = configuration;
    _context = context;
  }

  public bool Authorize( AuthDto authDto )
  {
    try
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes( _configuration["Jwt:Secret"] );
      tokenHandler.ValidateToken( authDto.Token, new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey( key ),
        ValidateIssuer = false,
        ValidateAudience = false
      }, out _ );
      return true;
    }
    catch
    {
      return false;
    }
  }

  public string? Login( LoginDto loginDto )
  {
    User? user = _context.Users.FirstOrDefault( u => u.Username == loginDto.Username );
    if ( user == null || !HashingHelper.VerifyPassword( loginDto.Password, user.PasswordHash ) )
    {
      throw new Exception( "Failed login" );
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
    if ( string.IsNullOrEmpty( secret ) )
    {
      throw new InvalidOperationException( "JWT secret is missing from configuration." );
    }

    var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( secret ) );

    var credentials = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );
    string userId = user.Id.ToString();
    var claimsList = new[]
    {
       new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() ),
       new Claim(JwtRegisteredClaimNames.Sub,  userId)
    };

    var token = new JwtSecurityToken(
      expires: DateTime.Now.AddHours( 24 ),
      signingCredentials: credentials
    );
    token.Payload.AddClaim( new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() ) );
    token.Payload.AddClaim( new Claim( JwtRegisteredClaimNames.Sub, userId ) );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
