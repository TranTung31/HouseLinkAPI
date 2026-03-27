using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace HouseLink.Identity.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
            => _logger = logger;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var name = typeof(TRequest).Name;
            _logger.LogInformation("[START] Handling {RequestName}", name);

            var sw = Stopwatch.StartNew();
            var response = await next();
            sw.Stop();

            if (sw.ElapsedMilliseconds > 500)
                _logger.LogWarning("[SLOW] {RequestName} took {Elapsed}ms — consider optimization", name, sw.ElapsedMilliseconds);
            else
                _logger.LogInformation("[END] {RequestName} completed in {Elapsed}ms", name, sw.ElapsedMilliseconds);

            return response;
        }
    }
}
