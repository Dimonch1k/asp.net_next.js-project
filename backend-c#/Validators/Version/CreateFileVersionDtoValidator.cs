using FluentValidation;
using backend_c_.DTO.Version;

namespace backend_c_.Validators.Version;

public class CreateFileVersionDtoValidator : AbstractValidator<CreateFileVersionDto>
{
  public CreateFileVersionDtoValidator( )
  {
    RuleFor( x => x.FileId )
      .GreaterThan( 0 ).WithMessage( "FileId must be a positive number." );

    RuleFor( x => x.VersionName )
      .NotEmpty().WithMessage( "VersionName is required." )
      .MaximumLength( 255 ).WithMessage( "VersionName cannot exceed 255 characters." );
  }
}