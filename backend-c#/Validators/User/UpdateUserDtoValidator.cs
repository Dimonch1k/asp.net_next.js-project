using FluentValidation;
using backend_c_.DTO.User;

namespace backend_c_.Validators.User;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
  public UpdateUserDtoValidator( )
  {
    RuleFor( x => x.Username )
      .NotEmpty().WithMessage( "Username is required." )
      .MinimumLength( 5 ).WithMessage( "Username must be at least 5 characters long." )
      .MaximumLength( 50 ).WithMessage( "Username cannot exceed 50 characters." );

    RuleFor( x => x.Email )
      .NotEmpty().WithMessage( "Email is required." )
      .EmailAddress().WithMessage( "Invalid email format." );

    RuleFor( x => x.FullName )
      .NotEmpty().WithMessage( "Full Name is required." )
      .MaximumLength( 100 ).WithMessage( "Full Name cannot exceed 100 characters." );
  }
}
