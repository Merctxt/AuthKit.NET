namespace AuthKit.Core.Services;

public interface IResult
{
    bool Success { get; }
    string Message { get; }
    string? Code { get; }
    
    static IResult SuccessResult(string message = "Operação realizada com sucesso.") => new Result(true, message, null);
    static IResult Failure(string message, string? code = null) => new Result(false, message, code);
}

public class Result : IResult
{
    public bool Success { get; }
    public string Message { get; }
    public string? Code { get; }
    
    public Result(bool success, string message, string? code)
    {
        Success = success;
        Message = message;
        Code = code;
    }
}
