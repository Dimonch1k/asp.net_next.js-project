using backend_c_.DTO.Access;
using FluentValidation;

namespace backend_c_.Validators.AccessLog;

public class UpdateAccessLogDtoValidator : AbstractValidator<UpdateAccessLogDto>
{
  public UpdateAccessLogDtoValidator( )
  {
    RuleFor( x => x.AccessType )
        .IsInEnum().WithMessage( "AccessType must be a valid enum value." );
  }
}
