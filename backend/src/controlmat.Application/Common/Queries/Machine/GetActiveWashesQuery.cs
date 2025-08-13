using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;
using System.Linq;

namespace Controlmat.Application.Common.Queries.Machine;

public static class GetActiveWashesQuery
{
    public record Request : IRequest<List<ActiveWashDto>>;

    public class Handler : IRequestHandler<Request, List<ActiveWashDto>>
    {
        private readonly IWashingRepository _repository;
        private readonly ILogger<Handler> _logger;

        public Handler(IWashingRepository repository, ILogger<Handler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<ActiveWashDto>> Handle(Request request, CancellationToken ct)
        {
            _logger.LogInformation("🌀 GetActiveWashesQuery - STARTED");

            var activeWashes = await _repository.GetActiveWashesAsync();
            var result = activeWashes.Select(w => new ActiveWashDto
            {
                MachineId = w.MachineId,
                WashingId = w.WashingId,
                StartDate = w.StartDate,
                StartUserName = w.StartUser?.UserName ?? "Unknown"
            }).ToList();

            _logger.LogInformation("✅ GetActiveWashesQuery - COMPLETED. Found {Count} active washes", result.Count);
            return result;
        }
    }
}

