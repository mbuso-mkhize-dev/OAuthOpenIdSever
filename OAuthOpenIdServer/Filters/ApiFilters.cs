using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OAuthOpenIdServer.ApplicationLogic.Exceptions;
using OAuthOpenIdServer.Models;
using System.Net;

namespace OAuthOpenIdServer.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var error = string.Empty;
            var errorDescription = string.Empty;

            if (context.Exception is ApplicationDuplicateDataException duplicateDataException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                error = "duplicate_data";
                errorDescription = string.IsNullOrWhiteSpace(duplicateDataException.Message) ? duplicateDataException.InnerException?.Message : duplicateDataException.Message;
            }
            else if (context.Exception is ApplicationIllegalOperationException illegalOperationException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                error = "illegal_operation";
                errorDescription = string.IsNullOrWhiteSpace(illegalOperationException.Message) ? illegalOperationException.InnerException?.Message : illegalOperationException.Message;
            }
            else if (context.Exception is ApplicationInvalidDataException invalidDataExpcetion)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                error = "invalid_data";
                errorDescription = string.IsNullOrWhiteSpace(invalidDataExpcetion.Message) ? invalidDataExpcetion.InnerException?.Message : invalidDataExpcetion.Message;
            }
            else if (context.Exception is ApplicationObjectNotFoundException objectNotFoundException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                error = "not_found";
                errorDescription = string.IsNullOrWhiteSpace(objectNotFoundException.Message) ? objectNotFoundException.InnerException?.Message : objectNotFoundException.Message;
            }
            else if (context.Exception is ApplicationUnauthorizedException unauthorizedException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                error = "unauthorized";
                errorDescription = string.IsNullOrWhiteSpace(unauthorizedException.Message) ? unauthorizedException.InnerException?.Message : unauthorizedException.Message;
            }
            else if (context.Exception is ApplicationForbiddenException forbiddenException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                error = "forbidden";
                errorDescription = string.IsNullOrWhiteSpace(forbiddenException.Message) ? forbiddenException.InnerException?.Message : forbiddenException.Message;
            }
            else if (context.Exception is ApplicationServerException applicationServerException)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                error = "server_error";
                errorDescription = string.IsNullOrWhiteSpace(applicationServerException.Message) ? applicationServerException.InnerException?.Message : applicationServerException.Message;
            }
            else
            {
                context.HttpContext.Response.StatusCode = 500;
#if DEBUG
                error = context.Exception.GetBaseException().Message;
                errorDescription = context.Exception.StackTrace;
#else
                error = "unknown_error";
                errorDescription = context.Exception.GetBaseException().Message;
#endif
            }

            context.Result = new JsonResult(new ApiErrorModel(error, errorDescription));

            base.OnException(context);
        }
    }
}