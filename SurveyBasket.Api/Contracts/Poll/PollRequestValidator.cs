using SurveyBasket.Api.Contracts.Poll.Request;

namespace SurveyBasket.Api.Contracts.Poll.Validations;

public class LoginRequestValidator : AbstractValidator<PollRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(p => p.Title).NotEmpty().Length(1, 100);
        RuleFor(p => p.Summary).NotEmpty().Length(1, 1500);

        RuleFor(p => p.StartsAt).NotEmpty().WithMessage("{PropertyName} is Required")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("StartsAt must be greater than or equal to today's date");

        RuleFor(p => p).Must(HasValidationsDates).WithMessage("EndsAt should be greter than startsAt date");

    }
    private bool HasValidationsDates(PollRequest poll)
    {
        return poll.EndsAt > poll.StartsAt;
    }
}
