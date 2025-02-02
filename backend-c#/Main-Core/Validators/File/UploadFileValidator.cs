using FluentValidation;
using backend_c_.DTO.File;
using backend_c_.Utils;

namespace backend_c_.Validators;

public class UploadFileValidator : AbstractValidator<UploadFileDto>
{
  public UploadFileValidator( )
  {
    RuleFor( x => x.UserId )
      .GreaterThan( 0 ).WithMessage( "UserId must be a positive number." );

    RuleFor( x => x.FileData )
      .NotNull().WithMessage( "FileData is required." )
      .Must( file => file != null && file.Length > 0 ).WithMessage( "FileSize must be a positive number." )
      .Must( file => file != null && file.Length <= 10485760 ).WithMessage( "FileSize cannot exceed 10 MB." );
  }
}
