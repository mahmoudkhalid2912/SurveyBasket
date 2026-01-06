namespace SurveyBasket.Api.Contracts.Results;

public record PollVotesResponse(string PollTitle,IEnumerable<VoteResponse>VoteResponses);
