using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;

namespace Controlmat.Application.Common.Queries.Washing;

public static class GetWashByMachineQuery
{
    public record Request(int MachineId) : IRequest<WashingResponseDto?>;

    public class Handler : IRequestHandler<Request, WashingResponseDto?>
    {
        private readonly IWashingRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(IWashingRepository repository, IMapper mapper, ILogger<Handler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<WashingResponseDto?> Handle(Request request, CancellationToken ct)
        {
            _logger.LogInformation("🌀 GetWashByMachineQuery - STARTED. MachineId: {MachineId}", request.MachineId);

            var washing = await _repository.GetActiveWashByMachineAsync(request.MachineId);
            if (washing == null)
            {
                _logger.LogInformation("ℹ️ No active wash found for machine {MachineId}", request.MachineId);
                return null;
            }

            var result = _mapper.Map<WashingResponseDto>(washing);
            _logger.LogInformation("✅ GetWashByMachineQuery - COMPLETED. Found wash: {WashingId}", washing.WashingId);
            return result;
        }
    }
}

