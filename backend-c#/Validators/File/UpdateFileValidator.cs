using FluentValidation;
using backend_c_.DTO.File;
using backend_c_.Utils;

namespace backend_c_.Validators;

public class UpdateFileValidator : AbstractValidator<UpdateFileDto>
{
  public UpdateFileValidator( )
  {
    RuleFor( x => x.FileName )
      .NotEmpty().WithMessage( "FileName is required." )
      .MaximumLength( 255 ).WithMessage( "FileName cannot exceed 255 characters." );
  }
}
