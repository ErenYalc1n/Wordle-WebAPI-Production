namespace Wordle.Application.Common.Exceptions;

public class InvalidSearchInputException : Exception
{
    public InvalidSearchInputException(string message) : base(message)
    {
    }
}
