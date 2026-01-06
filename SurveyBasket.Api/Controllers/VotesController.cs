using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace SurveyBasket.Api.Controllers;

[Route("api/Polls/{pollid}/vote")]
[ApiController]
[Authorize]
public class VotesController(IQuestionService questionService,IVoteService voteService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IVoteService _voteService = voteService;

    [HttpGet("")]
    public async Task<IActionResult> GetAvailableQuestions(int pollid, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _questionService.GetAvailableQuestionsAsync(pollid, userId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Vote(int pollid, [FromBody] VoteRequest voteRequest, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _voteService.AddAsync(pollid, userId, voteRequest, cancellationToken);
        return result.IsSuccess ? Created() : result.ToProblem();
    }
}
