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

    RuleFor( x => x.FileName )
      .NotEmpty().WithMessage( "FileName is required." )
      .MaximumLength( 255 ).WithMessage( "FileName cannot exceed 255 characters." );

    RuleFor( x => x.FilePath )
      .NotEmpty().WithMessage( "FilePath is required." )
      .MaximumLength( 500 ).WithMessage( "FilePath cannot exceed 500 characters." );

    RuleFor( x => x.FileSize )
      .GreaterThan( 0 ).WithMessage( "FileSize must be a positive number." )
      .LessThanOrEqualTo( 10485760 ).WithMessage( "FileSize cannot exceed 10 MB." );

    RuleFor( x => x.FileType )
      .NotEmpty().WithMessage( "FileType is required." )
      .Must( ValidationHelpers.BeAValidFileType ).WithMessage( "Invalid FileType." );
  }
}
