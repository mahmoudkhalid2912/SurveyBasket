namespace SurveyBasket.Api.Contracts.Poll.Request;

public record PollRequest(string Title,
    string Summary,
    DateOnly StartsAt,
    DateOnly EndsAt);


