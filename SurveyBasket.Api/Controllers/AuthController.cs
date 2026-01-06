
namespace SurveyBasket.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService,IOptions<JwtOptions> jwtoptions,ILogger<AuthController>logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;
    private readonly JwtOptions jwtoptions = jwtoptions.Value;

    [HttpPost("")]
    public async Task<IActionResult> LoginAsync(LoginRequest request,CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for {Email}{Password}", request.Email,request.Password);
        var authResponse = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

       return  authResponse.IsSuccess? Ok(authResponse.Value) : authResponse.ToProblem();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResponse = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return authResponse.IsSuccess ? Ok(authResponse) : authResponse.ToProblem();
    }

    [HttpPost("revoked-refresh")]
    public async Task<IActionResult> RevokedRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var IsRevoked = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return IsRevoked.IsSuccess ? Ok() : IsRevoked.ToProblem();
    }

}
