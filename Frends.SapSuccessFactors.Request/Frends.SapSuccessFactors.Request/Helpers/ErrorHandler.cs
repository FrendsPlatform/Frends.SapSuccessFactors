using System;
using System.Net.Http;
using Frends.SapSuccessFactors.Request.Definitions;

namespace Frends.SapSuccessFactors.Request.Helpers;

internal static class ErrorHandler
{
    internal static Result Handle(Exception exception, bool throwOnFailure, string errorMessageOnFailure)
    {
        if (throwOnFailure)
        {
            if (string.IsNullOrEmpty(errorMessageOnFailure))
                throw new Exception(exception.Message, exception);

            throw new Exception(errorMessageOnFailure, exception);
        }

        var errorMessage = !string.IsNullOrEmpty(errorMessageOnFailure)
            ? $"{errorMessageOnFailure}: {exception.Message}"
            : exception.Message;

        if (exception is HttpRequestException httpEx && httpEx.StatusCode.HasValue)
        {
            return new Result
            {
                Success = false,
                StatusCode = (int)httpEx.StatusCode.Value,
                Error = new Error { Message = errorMessage, AdditionalInfo = exception },
            };
        }

        return new Result { Success = false, Error = new Error { Message = errorMessage, AdditionalInfo = exception } };
    }
}