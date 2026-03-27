using FluentValidation;
using HouseLink.Identity.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace HouseLink.Identity.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                await WriteAsync(context, HttpStatusCode.BadRequest, new
                {
                    title = "Dữ liệu không hợp lệ",
                    status = 400,
                    errors
                });
            }
            catch (DomainException ex)
            {
                await WriteAsync(context, HttpStatusCode.BadRequest, new
                {
                    title = "Lỗi nghiệp vụ",
                    status = 400,
                    detail = ex.Message
                });
            }
            catch (NotFoundException ex)
            {
                await WriteAsync(context, HttpStatusCode.NotFound, new
                {
                    title = "Không tìm thấy",
                    status = 404,
                    detail = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteAsync(context, HttpStatusCode.Unauthorized, new
                {
                    title = "Không có quyền truy cập",
                    status = 401,
                    detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await WriteAsync(context, HttpStatusCode.InternalServerError, new
                {
                    title = "Lỗi hệ thống",
                    status = 500,
                    detail = "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại sau."
                });
            }
        }

        private static async Task WriteAsync(HttpContext ctx, HttpStatusCode code, object body)
        {
            ctx.Response.StatusCode = (int)code;
            ctx.Response.ContentType = "application/problem+json";
            var opts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(body, opts));
        }
    }
}
