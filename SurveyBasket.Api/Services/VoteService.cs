
using SurveyBasket.Api.Entites;

public class VoteService(ApplicationDbContext context) : IVoteService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> AddAsync(int PollId, string UserId, VoteRequest voteRequest, CancellationToken cancellationToken = default)
    {
        var hasVote = await _context.Votes
           .AnyAsync(v => v.PollId == PollId && v.UserId == UserId, cancellationToken);
        if (hasVote)
        {
            return Result.Failure<IEnumerable<VoteRequest>>(VoteError.UserAlreadyVoted);
        }
        var today = DateOnly.FromDateTime(DateTime.Now);
        var pollisExists = await _context.Polls.AnyAsync(p =>
            p.Id == PollId &&
            p.StartsAt <= today &&
            p.EndsAt >= today &&
            p.IsPublished,
            cancellationToken
        );

        if (!pollisExists)
        {
            return Result.Failure<VoteRequest>(PollErrors.PollNotFound);
        }
        var AvailableQuestions = await _context.Questions
            .Where(q => q.PollId == PollId && q.IsActive)
            .Select(q => q.Id)
            .ToListAsync(cancellationToken);

        if (!voteRequest.Answers.Select(x => x.QuestionId).SequenceEqual(AvailableQuestions))
            return Result.Failure(VoteError.InvalidQuestion);

        var Vote = new Vote
        {
            PollId = PollId,
            UserId = UserId,
            VoteAnswers = voteRequest.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };
        await _context.Votes.AddAsync(Vote, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
