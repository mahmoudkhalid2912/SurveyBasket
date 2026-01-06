namespace SurveyBasket.Api.Abstracions;

public class Result
{
    public Result(bool _IsSuccess, Error error)
    {
        if ((_IsSuccess && error != Error.None) || (!_IsSuccess && error == Error.None))
        {
            throw new InvalidOperationException();
        }
        IsSuccess = _IsSuccess;
        Error = error;
    }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    public static Result<TValue> Failure<TValue>(Error error) => new(default!, false, error);

}

public class Result<TValue>: Result
{
    private readonly TValue _value;
    public Result(TValue value,bool _IsSuccess, Error error):base(_IsSuccess,error)
    {
        _value = value;
    }
    public TValue Value=> IsSuccess ? _value : throw new InvalidOperationException();
}