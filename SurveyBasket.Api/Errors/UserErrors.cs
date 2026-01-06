
public static class UserErrors
{
    // User general
    public static readonly Error UserNotFound =
        new("User.NotFound", "The specified user was not found.", 404);

    public static readonly Error UserAlreadyExists =
        new("User.AlreadyExists", "A user with the given details already exists.", 400);

    public static readonly Error UnauthorizedAccess =
        new("User.UnauthorizedAccess", "You do not have permission to access this resource.", 403);

    // Login
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "The provided credentials are invalid.", 400);

    // Refresh token
    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "The refresh token is invalid or expired.", 401);

    public static readonly Error RefreshTokenAlreadyRevoked =
        new("User.RefreshTokenRevoked", "The refresh token has already been revoked.", 400);

    // Access token
    public static readonly Error InvalidToken =
        new("User.InvalidToken", "The access token is invalid or expired.", 401);

    // Registration / update
    public static readonly Error WeakPassword =
        new("User.WeakPassword", "The provided password does not meet the security requirements.", 400);

    public static readonly Error EmailNotConfirmed =
        new("User.EmailNotConfirmed", "The user's email has not been confirmed.", 400);

    // Optional extra
    public static readonly Error OperationFailed =
        new("User.OperationFailed", "The requested operation could not be completed.", 500);
}

