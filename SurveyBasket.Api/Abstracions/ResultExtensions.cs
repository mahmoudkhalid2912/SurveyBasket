public static class ResultExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert success result to problem");

        var error = result.Error;

        var problem = new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Description,
            Status = error.StatucCode
        };

        problem.Extensions["errors"] = new[]
        {
            new
            {
                error.Code,
                error.Description
            }
        };

        return new ObjectResult(problem)
        {
            StatusCode = error.StatucCode
        };
    }
}
