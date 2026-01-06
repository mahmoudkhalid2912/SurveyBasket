

namespace SurveyBasket.Api.Services;

public interface IResultService
{
    public Task<Result<PollVotesResponse>> GetPollVotesAsync(int PollId, CancellationToken cancellationToken);
    public Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int PollId, CancellationToken cancellationToken);
    public Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestion(int PollId, CancellationToken cancellationToken);
}
