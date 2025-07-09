namespace Wordle.Application.Common.Exceptions;

public class ValidationAppException : Exception
{
    public ValidationAppException(string message) : base(message) { }
}
