using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Loqora.Application.Common.Interfaces;

namespace Loqora.Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest>(ILogger<TRequest> logger, IUser user) : IRequestPreProcessor<TRequest> where TRequest : notnull
    {
        private readonly ILogger<TRequest> _logger = logger;
        private readonly IUser _user = user;

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _user.Id ?? string.Empty;

            _logger.LogInformation(
                "Request: {Name} {@UserId} {@Request}",
                requestName,
                userId,
                request
            );
        }
    }
}
