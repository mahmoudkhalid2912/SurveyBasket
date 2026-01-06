namespace SurveyBasket.Api.Contracts.Question;

public class VoteRequestValidator:AbstractValidator<QuestionRequest>
{
    public VoteRequestValidator()
    {
        RuleFor(q => q.Content).NotEmpty().Length(3, 1000);
        RuleFor(q => q.Answers).NotNull();
        RuleFor(q => q.Answers).Must(a => a.Count > 1).WithMessage("At least two answers are required.").When(q => q.Answers != null);

        RuleFor(q => q.Answers).Must(a => a.Distinct().Count() ==a.Count).WithMessage("Answers must be unique.").When(q => q.Answers != null);

    }
}
