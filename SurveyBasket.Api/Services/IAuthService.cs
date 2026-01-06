using SurveyBasket.Api.Abstracions;

public interface IAuthService
{
  
     Task<Result<AuthResponse>> GetTokenAsync(string email, string password,CancellationToken cancellationToken=default);
    Task<Result<AuthResponse>> GetRefreshTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default);
}
