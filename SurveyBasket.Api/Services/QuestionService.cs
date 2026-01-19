using Microsoft.Extensions.Caching.Hybrid;
using SurveyBasket.Api.Contracts.Answer;
using SurveyBasket.Api.Contracts.Question;

namespace SurveyBasket.Api.Services;

public class QuestionService(ApplicationDbContext context,HybridCache hybridCache) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly HybridCache _hybridCache = hybridCache;

    private const string QuestionCache= "AvailableQuestions";
    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken)
    {
        var pollisExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollisExists)
        {
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);
        }
        var questions = await _context.Questions
             .Where(q => q.PollId == pollId)
            .Include(q => q.Answers).ProjectToType<QuestionResponse>()
             .AsNoTracking()
            .ToListAsync(cancellationToken);
        return Result.Success(questions.Adapt<IEnumerable<QuestionResponse>>());
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableQuestionsAsync(int pollId,string userId, CancellationToken cancellationToken = default)
    {
        var hasVote = await _context.Votes
            .AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);
        if(hasVote)
        {
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteError.UserAlreadyVoted);
        }
        var today = DateOnly.FromDateTime(DateTime.Now); 
        var pollisExists = await _context.Polls.AnyAsync(p =>
            p.Id == pollId &&
            p.StartsAt <= today &&
            p.EndsAt >= today &&
            p.IsPublished,
            cancellationToken
        );

        if (!pollisExists)
        {
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);
        }

        var CacheKey = $"{QuestionCache}_{pollId}";
        var questions = await _hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(
        CacheKey,
        async entry =>
        {
            return await _context.Questions
                .Where(q => q.PollId == pollId && q.IsActive)
                .Include(q => q.Answers)
                .Select(q => new QuestionResponse(
                    q.Id,
                    q.Content,
                    q.Answers
                        .Where(a => a.IsActive)
                        .Select(a => new AnswerResponse(a.Id, a.Content))
                ))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
      
    );
 
        return Result.Success(questions.Adapt<IEnumerable<QuestionResponse>>());
    }
    public async Task<Result<QuestionResponse>> GetAsync(int pollId, int Id, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions
              .Where(q => q.PollId == pollId && q.Id == Id)
             .Include(q => q.Answers).ProjectToType<QuestionResponse>()
              .AsNoTracking()
             .SingleOrDefaultAsync(cancellationToken);
        if (question is null)
        {
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);
        }
        return Result<QuestionResponse>.Success(question);
    }
    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var pollisExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollisExists)
        {
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);
        }

        var QuestionIsExists = await _context.Questions
            .AnyAsync(q => q.Content == request.Content && q.PollId == pollId, cancellationToken);
        if (QuestionIsExists)
        {
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);
        }

        var question = request.Adapt<Question>();
        question.PollId = pollId;

        await _context.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveAsync($"{QuestionCache}_{pollId}");
        return Result.Success(question.Adapt<QuestionResponse>());
    }
    public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest questionRequest, CancellationToken cancellationToken = default)
    {
        var questionIsExists = await _context.Questions.AnyAsync(
            q => q.PollId == pollId
            && q.Id != id
            && q.Content == questionRequest.Content
            );
        if (questionIsExists)
            return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

        var question = await _context.Questions.
            Include(a => a.Answers).
            SingleOrDefaultAsync(q => q.PollId == pollId && q.Id == id);
        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);
        question.Content = questionRequest.Content;

        var CurrentAnswers = question.Answers.Select(x => x.Content).ToList();

        var NewAnswers = questionRequest.Answers.Except(CurrentAnswers).ToList();

        NewAnswers.ForEach(answer
            => question.Answers.Add(new Answer { Content = answer }));
        question.Answers.ToList().ForEach(answer =>
          answer.IsActive = questionRequest.Answers.Contains(answer.Content)
        );
        await _context.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveAsync($"{QuestionCache}_{pollId}");
        return Result.Success();

    }
    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions.SingleOrDefaultAsync(q => q.PollId == pollId && q.Id == id, cancellationToken);
        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;
        await _context.SaveChangesAsync();
        await _hybridCache.RemoveAsync($"{QuestionCache}_{pollId}");
        return Result.Success();
    }


  
}
