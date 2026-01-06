namespace SurveyBasket.Api.Errors;

public class QuestionErrors
{

    public static readonly Error QuestionNotFound = new("Question.NotFound", "The specified Question was not found.", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedQuestionContent = new("Question.QuestionContent", "Anthor Question with the same Content is already exists", StatusCodes.Status409Conflict);
}
