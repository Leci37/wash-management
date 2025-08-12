using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;

namespace Controlmat.Application.Common.Queries.Washing;

public static class GetWashByIdQuery
{
    public class Request : IRequest<WashingResponseDto?>
    {
        public long WashingId { get; set; }
    }

    public class Handler : IRequestHandler<Request, WashingResponseDto?>
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

        public async Task<WashingResponseDto?> Handle(Request request, CancellationToken cancellationToken)
        {
            var function = nameof(Handle);
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var washingId = request.WashingId;

            _logger.LogInformation("🌀 {Function} [Thread:{ThreadId}] - STARTED. WashingId: {WashingId}",
                function, threadId, washingId);

            try
            {
                var washing = await _washingRepo.GetByIdWithDetailsAsync(washingId);
                if (washing == null)
                {
                    _logger.LogWarning("⚠️ {Function} [Thread:{ThreadId}] - WASHING NOT FOUND. WashingId: {WashingId}",
                        function, threadId, washingId);
                    return null;
                }

                var result = _mapper.Map<WashingResponseDto>(washing);

                _logger.LogInformation("✅ {Function} [Thread:{ThreadId}] - COMPLETED. WashingId: {WashingId}, Status: {Status}, ProtCount: {ProtCount}, PhotoCount: {PhotoCount}",
                    function, threadId, washingId, result.Status, result.Prots.Count, result.Photos.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ {Function} [Thread:{ThreadId}] - ERROR. WashingId: {WashingId}",
                    function, threadId, washingId);
                throw;
            }
        }
    }
}
