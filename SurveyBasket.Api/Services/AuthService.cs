using SurveyBasket.Api.Abstracions;
using SurveyBasket.Api.Errors;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager,IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly int _refreshTokenExpiryDays = 14;
      public async  Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null) return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        bool isPasswordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isPasswordValid) return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var (Token, ExpiresIn) = _jwtProvider.GenerateJwtToken(user);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration,
        });
        await _userManager.UpdateAsync(user);
        var authResponse = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, Token, ExpiresIn, refreshToken, refreshTokenExpiration);
        return Result.Success(authResponse);
    }



    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken)
    {
        var userid = _jwtProvider.ValidateToken(token);
        if (userid is null) return Result.Failure<AuthResponse>(UserErrors.InvalidToken);
        var user = _userManager.FindByIdAsync(userid).Result;
        if (user is null) return Result.Failure<AuthResponse>(UserErrors.UserNotFound);
        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshtoken && rt.IsActive);
        if (userRefreshToken is null) return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);
        userRefreshToken.RevokedOn = DateTime.UtcNow;
        var (NewToken, ExpiresIn) = _jwtProvider.GenerateJwtToken(user);

        var NewrefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = NewrefreshToken,
            ExpiresOn = refreshTokenExpiration,
        });
        await _userManager.UpdateAsync(user);
        var authResponse = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, NewToken, ExpiresIn, NewrefreshToken, refreshTokenExpiration);
        return Result.Success(authResponse);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default)
    {
        var userid = _jwtProvider.ValidateToken(Token);
        if (userid is null) return Result.Failure(UserErrors.InvalidToken);
        var user = _userManager.FindByIdAsync(userid).Result;
        if (user is null) return Result.Failure(UserErrors.UserNotFound);
        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == RefreshToken && rt.IsActive);
        if (userRefreshToken is null) return Result.Failure(UserErrors.RefreshTokenAlreadyRevoked);
        userRefreshToken.RevokedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        return Result.Success();
    }

    private static string GenerateRefreshToken()
    {

        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

   
    
}
