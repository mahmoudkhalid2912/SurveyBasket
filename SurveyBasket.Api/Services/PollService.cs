


using Azure.Core;
using SurveyBasket.Api.Contracts.Poll.Request;
using SurveyBasket.Api.Entites;

namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDbContext context) : IPoolService
{
    private readonly ApplicationDbContext _context = context;
    public async Task<IEnumerable<PollResponse>> GetAllAsync
        (CancellationToken cancellationToken) => await _context.Polls
        .AsNoTracking()
        .ProjectToType<PollResponse>()
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PollResponse>> GetCurrentAsync(CancellationToken cancellationToken = default)=>
        await _context.Polls
        .AsNoTracking().Where(p => p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow)&&p.IsPublished)
        .ProjectToType<PollResponse>()
        .ToListAsync(cancellationToken);

    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);
        return poll is null ?
            Result.Failure<PollResponse>(PollErrors.PollNotFound) : 
            Result.Success<PollResponse>(poll.Adapt<PollResponse>()); 
    }


    public async Task<Result<PollResponse>> AddAsync(PollRequest pollRequest, CancellationToken cancellationToken = default)
    {
        bool isExsistingTitle = await _context.Polls
                .AnyAsync(e => e.Title.Trim().ToLower() == pollRequest.Title.Trim().ToLower(), cancellationToken);

        if (isExsistingTitle)
            return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);

        var entity =pollRequest.Adapt<Poll>();
        await _context.Polls.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success<PollResponse>(entity.Adapt<PollResponse>());
    }


    public async Task<Result> UpdateAsync(int id, PollRequest pollRequest, CancellationToken cancellationToken)
    {
        var currentPoll = await _context.Polls.FindAsync(id, cancellationToken);
        if (currentPoll is null)
        {
            return Result.Failure(PollErrors.PollNotFound);
        }

        bool isExsistingTitle = await _context.Polls
                .AnyAsync(e => e.Title.Trim().ToLower() == pollRequest.Title.Trim().ToLower()&&e.Id!=id, cancellationToken);

        if (isExsistingTitle)
            return Result.Failure(PollErrors.DuplicatedPollTitle);

        
        currentPoll.Title = pollRequest.Title;
        currentPoll.Summary = pollRequest.Summary;
        currentPoll.StartsAt = pollRequest.StartsAt;
        currentPoll.EndsAt = pollRequest.EndsAt;
        _context.Polls.Update(currentPoll);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();

    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var Poll = await GetAsync(id, cancellationToken);
        if (Poll is null)
        {
            return Result.Failure(PollErrors.PollNotFound);
        }
        _context.Remove(Poll);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ToggleIsPublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var Poll = await _context.Polls.FindAsync(id, cancellationToken);
        if(Poll is null) return Result.Failure(PollErrors.PollNotFound);
        _context.Attach(Poll);
        Poll.IsPublished = !Poll.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
