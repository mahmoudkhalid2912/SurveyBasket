namespace SurveyBasket.Api.Authentication;

public interface IJwtProvider
{
    (string Token,int ExpiresIn) GenerateJwtToken(ApplicationUser user);

    string? ValidateToken(string Token);
}
