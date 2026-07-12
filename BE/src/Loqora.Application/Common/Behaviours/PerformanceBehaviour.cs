
using System.Diagnostics;

using Loqora.Application.Common.Interfaces;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Loqora.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse>
    (ILogger<TRequest> logger, IUser user) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer = new();
    private readonly ILogger<TRequest> _logger = logger;
    private readonly IUser _user = user;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        _timer.Start();

        var response = await next(ct);

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _user.Id ?? string.Empty;

            _logger.LogWarning(
               "Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserName} {@Request}",
               requestName,
               elapsedMilliseconds,
               userId,
               request);
        }

        return response;
    }
}
