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
            catch (InvalidDataTypeException ex)
            {
                await HandleExceptionAsync(context, ex);
                errorLogger.WriteLogError($"{ex}");
            }
            catch (TruckOverloadedException ex)
            {
                await HandleExceptionAsync(context, ex);
                errorLogger.WriteLogError($"{ex}");
            }
            catch (InternalServerException ex)
            {
                await HandleExceptionAsync(context, ex);
                errorLogger.WriteLogError($"{ex}");
            }
            catch (InsufficientQuantityException ex)
            {
                await HandleExceptionAsync(context, ex);
                errorLogger.WriteLogError($"{ex}");
            }
            catch (InvalidEmployeeException ex)
            {
                await HandleExceptionAsync(context, ex);
                errorLogger.WriteLogError($"{ex}");
            }
            catch (NotFoundException ex)
            {
                await HandleExceptionAsync(context, ex);
                errorLogger.WriteLogError($"{ex}");
            }
            catch (InvalidTruckException ex)
            {
                await HandleExceptionAsync(context, ex);
                errorLogger.WriteLogError($"{ex}");
            }
            catch (UnregisteredObject ex)
            {
                await HandleExceptionAsync(context, ex);
                errorLogger.WriteLogError($"{ex}");
            }
            catch (DatabaseConnectionException ex)
            {
                await HandleExceptionAsync(context, ex);
                errorLogger.WriteLogError($"{ex}");
            }
            catch (NullRequestException ex)
            {
                await HandleExceptionAsync(context, ex);
                errorLogger.WriteLogError($"{ex}");
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
                errorLogger.WriteLogError($"{ex}");
            }
            
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string stackTrace = string.Empty;
            string message;
            var exceptionType = exception.GetType();

            switch (exceptionType.Name)
            {
                case nameof(InvalidDataTypeException):
                    message = exception.Message;
                    status = HttpStatusCode.BadRequest;
                    stackTrace = exception.StackTrace;
                    break;

                case nameof(NotFoundException):
                    message = exception.Message;
                    status = HttpStatusCode.NotFound;
                    stackTrace = exception.StackTrace;
                    break;

                case nameof(TruckOverloadedException):
                    message = exception.Message;
                    status = HttpStatusCode.BadRequest;
                    stackTrace = exception.StackTrace;
                    break;

                case nameof(InternalServerException):
                    message = exception.Message;
                    status = HttpStatusCode.InternalServerError;
                    stackTrace = exception.StackTrace;
                    break;

                case nameof(InvalidTruckException):
                    message = exception.Message;
                    status = HttpStatusCode.BadRequest;
                    stackTrace = exception.StackTrace;
                    break;

                case nameof(InvalidEmployeeException):
                    message = exception.Message;
                    status = HttpStatusCode.BadRequest;
                    stackTrace = exception.StackTrace;
                    break;

                case nameof(InsufficientQuantityException):
                    message = exception.Message;
                    status = HttpStatusCode.InsufficientStorage;
                    stackTrace = exception.StackTrace;
                    break;

                case nameof(UnregisteredObject):
                    message = exception.Message;
                    status = HttpStatusCode.NotFound;
                    stackTrace = exception.StackTrace;
                    break;

                case nameof(DatabaseConnectionException):
                    message = exception.Message;
                    status = HttpStatusCode.InternalServerError;
                    stackTrace = exception.StackTrace;
                    break;

                default:
                    message = exception.Message;
                    status = HttpStatusCode.InsufficientStorage;
                    stackTrace = exception.StackTrace;
                    break;
            }

            var result = JsonSerializer.Serialize(new {status, message, stackTrace });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(result);

        }


    }
}
