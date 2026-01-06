namespace SurveyBasket.Api.Authentication;

public class JwtProvider(IOptions<JwtOptions> jwtoptions) : IJwtProvider
{
    private readonly JwtOptions _options = jwtoptions.Value;

    public (string Token, int ExpiresIn) GenerateJwtToken(ApplicationUser user)
    {
        Claim[] claims = [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];

        var SymmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        var signingCredentials = new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);

       
        var Token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
            signingCredentials: signingCredentials
            );

        return (Token: new JwtSecurityTokenHandler().WriteToken(Token), ExpiresIn:_options.ExpiryMinutes);
    }

    public string? ValidateToken(string Token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var SymmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        try
        {
            tokenHandler.ValidateToken(Token,new TokenValidationParameters
            {
                IssuerSigningKey = SymmetricSecurityKey,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero

            }, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken) validatedToken;
            return jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        }
        catch
        {
            return null;
        }
    }
}
