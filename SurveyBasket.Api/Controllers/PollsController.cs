
namespace SurveyBasket.Api.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PollsController(IPoolService poolService) : ControllerBase
    {
        private readonly IPoolService _pollService = poolService;



        [HttpGet("")]

        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            return Ok( await _pollService.GetAllAsync(cancellationToken)); 
        }
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            return Ok(await _pollService.GetCurrentAsync(cancellationToken));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
        {
            var pollResponse = await _pollService.GetAsync(id, cancellationToken);
            return pollResponse.IsSuccess ? Ok(pollResponse.Value) : pollResponse.ToProblem();
        }

        [HttpPost("")]
        public async Task<IActionResult> Add(PollRequest request)
        {
            var result = await _pollService.AddAsync(request);
            return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value) : result.ToProblem();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PollRequest request)
        {
            var result = await _pollService.UpdateAsync(id, request);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _pollService.DeleteAsync(id);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("{id}/toggle-publish")]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var result = await _pollService.ToggleIsPublishStatusAsync(id);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }


    }
}
