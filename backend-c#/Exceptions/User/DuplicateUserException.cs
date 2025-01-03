namespace backend_c_.Exceptions.User;

public class DuplicateUserException : AppException
{
  public DuplicateUserException( string username )
      : base( $"A user with username '{username}' already exists." ) { }

  public DuplicateUserException( string email, bool isEmail )
      : base( $"A user with email '{email}' already exists." ) { }
}

