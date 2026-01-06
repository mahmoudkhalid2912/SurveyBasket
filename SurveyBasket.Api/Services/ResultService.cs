
using System.Collections.Generic;

namespace SurveyBasket.Api.Services;

public class ResultService(ApplicationDbContext context) : IResultService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int PollId, CancellationToken cancellationToken)
    {
        var PollVotes = await _context.Polls
            .Where(x => x.Id == PollId)
            .Select(p => new PollVotesResponse(
                p.Title,
                p.Votes.Select(v => new VoteResponse(
                   $"{v.User.FirstName} {v.User.LastName}",
                   v.SubmittedOn,
                   v.VoteAnswers.Select(va => new QuestionAnswerResponse(
                        va.Question.Content,
                        va.Answer.Content
                )
            )
                   )
                )
                )
            ).SingleOrDefaultAsync(cancellationToken);

        return PollVotes is null ? Result.Failure<PollVotesResponse>(PollErrors.PollNotFound)
            : Result.Success(PollVotes);
    }
    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int PollId, CancellationToken cancellationToken)
    {
        var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == PollId, cancellationToken);
        if (!pollIsExist)
        {
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);
        }
        var votesPerDay = await _context.Votes.Where(v => v.PollId == PollId)
            .GroupBy(v => new { Date = DateOnly.FromDateTime(v.SubmittedOn) })
            .Select(g => new VotesPerDayResponse(g.Key.Date, g.Count())).ToListAsync(cancellationToken);
        return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);
    }
    public async Task<Result<IEnumerable<VotesPerQuestionResponse>>>GetVotesPerQuestion(int PollId,CancellationToken cancellationToken)
    {
        var pollIsExist = await _context.Polls.AnyAsync(p => p.Id == PollId, cancellationToken);
        if (!pollIsExist)
        {
            return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollNotFound);
        }

        var VotesPerQuestion = await _context.VoteAnswers
            .Where(v => v.Vote.PollId == PollId)
            .Select(x => new VotesPerQuestionResponse(
                x.Question.Content,
                x.Question.VoteAnswers.GroupBy(va => new { AnswerID = va.AnswerId, AnswerContent = va.Answer.Content }).Select(g => new VotesPerAnswersResponse(
                    g.Key.AnswerContent,
                    g.Count()
                )))).ToListAsync(cancellationToken);
        return Result.Success<IEnumerable<VotesPerQuestionResponse>>(VotesPerQuestion);
    }
}
