using AwsLambda.Core.Shared.ErrorCodes;

namespace AwsLambda.Core.Exceptions;

public class AlreadyExistException : AppException
{
    public AlreadyExistException()
        : base(ApiResultStatusCode.NotFound, System.Net.HttpStatusCode.NotFound)
    {
    }

    public AlreadyExistException(string message)
        : base(ApiResultStatusCode.NotFound, message, System.Net.HttpStatusCode.NotFound)
    {
    }

    public AlreadyExistException(object additionalData)
        : base(ApiResultStatusCode.NotFound, null, System.Net.HttpStatusCode.NotFound, additionalData)
    {
    }

    public AlreadyExistException(string message, object additionalData)
        : base(ApiResultStatusCode.NotFound, message, System.Net.HttpStatusCode.NotFound, additionalData)
    {
    }

    public AlreadyExistException(string message, Exception exception)
        : base(ApiResultStatusCode.NotFound, message, exception, System.Net.HttpStatusCode.NotFound)
    {
    }

    public AlreadyExistException(string message, Exception exception, object additionalData)
        : base(ApiResultStatusCode.NotFound, message, System.Net.HttpStatusCode.NotFound, exception, additionalData)
    {
    }
}
