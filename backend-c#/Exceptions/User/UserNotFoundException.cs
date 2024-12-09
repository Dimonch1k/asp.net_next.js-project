namespace backend_c_.Exceptions.User;

public class UserNotFoundException : AppException
{
    public UserNotFoundException(int userId)
        : base($"User with ID {userId} was not found.") { }

    public UserNotFoundException(string username)
        : base($"User with username '{username}' was not found.") { }
}
