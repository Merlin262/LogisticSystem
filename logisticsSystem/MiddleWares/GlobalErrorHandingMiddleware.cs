using System.Net;
using logisticsSystem.Data;
using logisticsSystem.Exceptions;
using logisticsSystem.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace logisticsSystem.MiddleWares
{
    public class GlobalErrorHandingMiddleware
    {
        private readonly RequestDelegate _next;


        public GlobalErrorHandingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, LoggerService errorLogger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, errorLogger); // Passando o objeto LoggerService como parâmetro
            }
        }


        private static Task HandleExceptionAsync(HttpContext context, Exception exception, LoggerService logger)
        {
            HttpStatusCode status;
            string message;
            var exceptionType = exception.GetType();

            switch (exceptionType.Name)
            {
                case nameof(InvalidDataTypeException):
                    message = exception.Message;
                    status = HttpStatusCode.BadRequest;
                    break;

                case nameof(NotFoundException):
                    message = exception.Message;
                    status = HttpStatusCode.NotFound;
                    break;

                case nameof(TruckOverloadedException):
                    message = exception.Message;
                    status = HttpStatusCode.BadRequest;
                    break;

                case nameof(InternalServerException):
                    message = exception.Message;
                    status = HttpStatusCode.InternalServerError;
                    break;

                case nameof(InvalidTruckException):
                    message = exception.Message;
                    status = HttpStatusCode.BadRequest;
                    break;

                case nameof(InvalidEmployeeException):
                    message = exception.Message;
                    status = HttpStatusCode.BadRequest;
                    break;

                case nameof(InsufficientQuantityException):
                    message = exception.Message;
                    status = HttpStatusCode.InsufficientStorage;
                    break;

                case nameof(UnregisteredObjectException):
                    message = exception.Message;
                    status = HttpStatusCode.NotFound;
                    break;

                case nameof(DatabaseConnectionException):
                    message = exception.Message;
                    status = HttpStatusCode.InternalServerError;
                    break;

                default:
                    message = exception.Message;
                    status = HttpStatusCode.InternalServerError;
                    break;
            }

            // Obter o stack trace
            string stackTrace = exception.StackTrace;

            // Registrar o erro no log
            logger.WriteLogError($"\nStatus: {status},\n Message: {message},\n StackTrace: {stackTrace}\n");

            // Construir o resultado JSON sem incluir o stack trace
            var result = JsonSerializer.Serialize(new { status, message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(result);
        }

    }
}
