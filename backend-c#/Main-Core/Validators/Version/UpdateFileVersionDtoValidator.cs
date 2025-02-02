using FluentValidation;
using backend_c_.DTO.Version;

namespace backend_c_.Validators.Version;

public class UpdateFileVersionDtoValidator : AbstractValidator<UpdateFileVersionDto>
{
  public UpdateFileVersionDtoValidator( )
  {
    RuleFor( x => x.VersionName )
      .NotEmpty().WithMessage( "VersionName is required." )
      .MaximumLength( 255 ).WithMessage( "VersionName cannot exceed 255 characters." );
  }
}
