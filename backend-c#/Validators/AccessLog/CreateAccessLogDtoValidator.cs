using backend_c_.DTO.Access;
using FluentValidation;

namespace backend_c_.Validators.AccessLog;

public class CreateAccessLogDtoValidator : AbstractValidator<CreateAccessLogDto>
{
  public CreateAccessLogDtoValidator( )
  {
    RuleFor( x => x.FileId )
        .GreaterThan( 0 ).WithMessage( "FileId must be a positive integer." );

    RuleFor( x => x.UserId )
        .GreaterThan( 0 ).WithMessage( "UserId must be a positive integer." );

    RuleFor( x => x.AccessType )
        .IsInEnum().WithMessage( "AccessType must be a valid enum value." );
  }
}
