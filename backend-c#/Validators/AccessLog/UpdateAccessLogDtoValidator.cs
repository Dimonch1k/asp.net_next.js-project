using backend_c_.DTO.Access;
using backend_c_.Utils;
using FluentValidation;

namespace backend_c_.Validators.AccessLog;

public class UpdateAccessLogDtoValidator : AbstractValidator<UpdateAccessLogDto>
{
  public UpdateAccessLogDtoValidator( )
  {
    RuleFor( x => x.AccessType )
    .Must( x => ValidationHelpers.BeAValidAccessType( x.ToLower() ) )
    .WithMessage( "Invalid access type. Allowed values: read, write, download." );
  }
}
