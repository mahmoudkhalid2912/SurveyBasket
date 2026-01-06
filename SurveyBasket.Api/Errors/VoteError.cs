namespace SurveyBasket.Api.Errors;

public class VoteError
{
    public static readonly Error UserAlreadyVoted = new(
        "Vote.UserAlreadyVoted",
        "The user has already voted in this poll.", StatusCodes.Status409Conflict);

    public static readonly Error InvalidQuestion= new(
        "Vote.InvalidQuestion",
        "One or more questions in the vote request are invalid.",
        StatusCodes.Status400BadRequest);
}
