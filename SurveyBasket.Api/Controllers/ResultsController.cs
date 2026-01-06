using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Api.Controllers;

[Route("api/Polls/{pollid}/[controller]")]
[ApiController]
[Authorize]
public class ResultsController(IResultService resultService) : ControllerBase
{
    private readonly IResultService _resultService = resultService;
    [HttpGet("raw-data")]
    public async Task<IActionResult> PollVotes(int pollId, CancellationToken cancellationToken)
    {
        var rawData = await _resultService.GetPollVotesAsync(pollId, cancellationToken);
        return rawData.IsSuccess ?
             Ok(rawData.Value)
            : rawData.ToProblem();
    }
    [HttpGet("votes-per-day")]
    public async Task<IActionResult> votesperDay(int pollId, CancellationToken cancellationToken)
    {
        var rawData = await _resultService.GetVotesPerDayAsync(pollId, cancellationToken);
        return rawData.IsSuccess ?
             Ok(rawData.Value)
            : rawData.ToProblem();
    }
    [HttpGet("votes-per-question")]
    public async Task<IActionResult> VotesPerQuestion(int pollId, CancellationToken cancellationToken)
    {
        var rawData = await _resultService.GetVotesPerQuestion(pollId, cancellationToken);
        return rawData.IsSuccess ?
             Ok(rawData.Value)
            : rawData.ToProblem();
    }
}
