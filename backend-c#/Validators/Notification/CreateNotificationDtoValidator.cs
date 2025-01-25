using backend_c_.DTO.Notification;
using FluentValidation;

namespace backend_c_.Validators.Notification;

public class CreateNotificationDtoValidator : AbstractValidator<CreateNotificationDto>
{
  public CreateNotificationDtoValidator( )
  {
    RuleFor( x => x.UserId )
      .GreaterThan( 0 ).WithMessage( "UserId must be a positive number." );
  }
}
