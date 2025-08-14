using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Controlmat.Application.Common.Constants;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;

namespace Controlmat.Application.Common.Queries.Washing;

public static class GetWashByMachineQuery
{
    public record Request(int MachineId) : IRequest<WashingResponseDto?>;

    public class Handler : IRequestHandler<Request, WashingResponseDto?>
    {
        private readonly IWashingRepository _repository;
        private readonly IMachineRepository _machineRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IWashingRepository repository,
            IMachineRepository machineRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _repository = repository;
            _machineRepository = machineRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<WashingResponseDto?> Handle(Request request, CancellationToken ct)
        {
            _logger.LogInformation("🌀 GetWashByMachineQuery - STARTED. MachineId: {MachineId}", request.MachineId);

            if (request.MachineId < 1 || request.MachineId > 4)
            {
                throw new ValidationException(ValidationErrorMessages.Machine.InvalidRange(request.MachineId));
            }

            if (!await _machineRepository.ExistsAsync(request.MachineId))
            {
                throw new ValidationException(ValidationErrorMessages.Machine.NotFound(request.MachineId));
            }

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

