using backend_c_.DTO.Access;
using backend_c_.Utils;
using FluentValidation;

namespace backend_c_.Validators.AccessLog;

public class CreateAccessLogDtoValidator : AbstractValidator<CreateAccessLogDto>
{
  public CreateAccessLogDtoValidator( )
  {
    RuleFor( x => x.SharedFileId )
      .GreaterThan( 0 ).WithMessage( "SharedFileId must be a positive integer." );

    RuleFor( x => x.UserId )
      .GreaterThan( 0 ).WithMessage( "UserId must be a positive integer." );

    RuleFor( x => x.AccessType )
      .Must( x => ValidationHelpers.BeAValidAccessType( x.ToLower() ) )
      .WithMessage( "Invalid access type. Allowed values: read, write, download." );
  }
}
