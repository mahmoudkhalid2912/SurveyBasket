
public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(rf => rf.RefreshToken).NotEmpty();
        RuleFor(rf=> rf.Token).NotEmpty();
    }

}