using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;

namespace Controlmat.Application.Common.Queries.Machine;

public static class GetMachinesQuery
{
    public class Request : IRequest<List<MachineDto>> { }

    public class Handler : IRequestHandler<Request, List<MachineDto>>
    {
        private readonly IMachineRepository _machineRepo;
        private readonly IWashingRepository _washingRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IMachineRepository machineRepo,
            IWashingRepository washingRepo,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _machineRepo = machineRepo;
            _washingRepo = washingRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<MachineDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var function = nameof(Handle);
            var threadId = Thread.CurrentThread.ManagedThreadId;

            _logger.LogInformation("🌀 {Function} [Thread:{ThreadId}] - STARTED", function, threadId);

            try
            {
                var machines = await _machineRepo.GetAllAsync();
                var result = _mapper.Map<List<MachineDto>>(machines);

                foreach (var machine in result)
                {
                    machine.IsAvailable = !await _washingRepo.IsMachineInUseAsync(machine.Id);
                }

                _logger.LogInformation("✅ {Function} [Thread:{ThreadId}] - COMPLETED. MachineCount: {Count}",
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
