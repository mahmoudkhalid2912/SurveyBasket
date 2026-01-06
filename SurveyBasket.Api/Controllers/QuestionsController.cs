using SurveyBasket.Api.Contracts.Question;

namespace SurveyBasket.Api.Controllers;

[Route("api/Polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(int pollId, CancellationToken cancellationToken = default)
    {
        var result = await _questionService.GetAllAsync(pollId, cancellationToken);
        return result.IsSuccess
       ? Ok(result.Value)
       : result.ToProblem();
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> Get(int pollId, int Id, CancellationToken cancellationToken = default)
    {
        var result = await _questionService.GetAsync(pollId, Id, cancellationToken);
        return result.IsSuccess
       ? Ok(result.Value)
       : result.ToProblem();
    }
    [HttpPost("")]
    public async Task<IActionResult> Add(int pollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _questionService.AddAsync(pollId, request, cancellationToken);
        return result.IsSuccess
       ? CreatedAtAction(nameof(Get), new { pollId, result.Value.Id }, result.Value)
       : result.ToProblem();

    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int pollId, int id,[FromBody]QuestionRequest questionRequest, CancellationToken cancellationToken)
    {
        var result = await _questionService.UpdateAsync(pollId, id, questionRequest, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpPut("{id}/toggleStatus")]
    public async Task<IActionResult>ToggleStatus(int pollId,int id,CancellationToken cancellationToken)
    {
        var result = await _questionService.ToggleStatusAsync(pollId, id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
