
public interface IVoteService
{
    Task<Result> AddAsync(int PollId, string UserId, VoteRequest voteRequest, CancellationToken cancellationToken=default!);
}
