using System;
using System.Net;

namespace Wordle.Application.Common.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
    public HttpStatusCode StatusCode => HttpStatusCode.Conflict;
}
