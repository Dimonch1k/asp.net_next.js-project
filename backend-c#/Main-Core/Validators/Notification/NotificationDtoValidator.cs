using backend_c_.DTO.Notification;
using FluentValidation;

namespace backend_c_.Validators.Notification;

public class NotificationDtoValidator : AbstractValidator<NotificationDto>
{
  public NotificationDtoValidator( )
  {
    RuleFor( x => x.Id )
      .GreaterThan( 0 ).WithMessage( "Id must be a positive number." );

    RuleFor( x => x.UserId )
      .GreaterThan( 0 ).WithMessage( "UserId must be a positive number." );
  }
}
