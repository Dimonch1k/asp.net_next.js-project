using FluentValidation;
using backend_c_.DTO.User;

namespace backend_c_.Validators.User;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
  public LoginDtoValidator( )
  {
    RuleFor( x => x.Username )
      .NotEmpty().WithMessage( "Username is required." );

    RuleFor( x => x.Password )
      .NotEmpty().WithMessage( "Password is required." );
  }
}
