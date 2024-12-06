using FluentValidation;
using backend_c_.DTO.SharedFile;

namespace backend_c_.Validators.SharedFile;

public class SharedFileDtoValidator : AbstractValidator<ShareFileDto>
{
  public SharedFileDtoValidator( )
  {
    RuleFor( x => x.FileId )
      .GreaterThan( 0 ).WithMessage( "FileId must be a positive number." );

    RuleFor( x => x.OwnerId )
      .GreaterThan( 0 ).WithMessage( "OwnerId must be a positive number." );

    RuleFor( x => x.SharedWithId )
      .GreaterThan( 0 ).WithMessage( "SharedWithId must be a positive number." );
  }
}
