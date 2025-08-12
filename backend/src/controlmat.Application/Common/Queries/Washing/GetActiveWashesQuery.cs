using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;

namespace Controlmat.Application.Common.Queries.Washing;

public static class GetActiveWashesQuery
{
    public class Request : IRequest<List<ActiveWashDto>> { }

    public class Handler : IRequestHandler<Request, List<ActiveWashDto>>
    {
        private readonly IWashingRepository _washingRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IWashingRepository washingRepo,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _washingRepo = washingRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ActiveWashDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var function = nameof(Handle);
            var threadId = Thread.CurrentThread.ManagedThreadId;

            _logger.LogInformation("🌀 {Function} [Thread:{ThreadId}] - STARTED", function, threadId);

            try
            {
                var activeWashes = await _washingRepo.GetActiveWashesAsync();
                var result = _mapper.Map<List<ActiveWashDto>>(activeWashes);

                _logger.LogInformation("✅ {Function} [Thread:{ThreadId}] - COMPLETED. ActiveWashCount: {Count}",
                    function, threadId, result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ {Function} [Thread:{ThreadId}] - ERROR", function, threadId);
                throw;
            }
        }
    }
}
