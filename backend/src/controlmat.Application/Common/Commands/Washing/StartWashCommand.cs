using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FluentValidation;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;

namespace Controlmat.Application.Common.Commands.Washing
{
    public static class StartWashCommand
    {
        public class Request : IRequest<WashingResponseDto>
        {
            public NewWashDto Dto { get; set; } = new();
        }

        public class Handler : IRequestHandler<Request, WashingResponseDto>
        {
            private readonly IWashingRepository _washingRepo;
            private readonly IProtRepository _protRepo;
            private readonly IUserRepository _userRepo;
            private readonly IMachineRepository _machineRepo;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(
                IWashingRepository washingRepo,
                IProtRepository protRepo,
                IUserRepository userRepo,
                IMachineRepository machineRepo,
                IMapper mapper,
                ILogger<Handler> logger)
            {
                _washingRepo = washingRepo;
                _protRepo = protRepo;
                _userRepo = userRepo;
                _machineRepo = machineRepo;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<WashingResponseDto> Handle(Request request, CancellationToken ct)
            {
                var function = nameof(Handle);
                var threadId = Thread.CurrentThread.ManagedThreadId;

                _logger.LogInformation("\ud83d\udd00 {Function} [Thread:{ThreadId}] - STARTED. Input: {@Request}",
                    function, threadId, request.Dto);

                try
                {
                    // Business Rule 1: Max 2 active washes
                    var activeCount = await _washingRepo.CountActiveAsync();
                    if (activeCount >= 2)
                    {
                        throw new ValidationException("Maximum 2 active washes allowed");
                    }

                    // Business Rule 2: Machine not in use
                    var machineInUse = await _washingRepo.IsMachineInUseAsync(request.Dto.MachineId);
                    if (machineInUse)
                    {
                        throw new ValidationException($"Machine {request.Dto.MachineId} is already in use");
                    }

                    // Validate user exists
                    var user = await _userRepo.GetByIdAsync(request.Dto.StartUserId);
                    if (user == null)
                    {
                        throw new ValidationException($"User {request.Dto.StartUserId} not found");
                    }

                    // Validate machine exists
                    var machine = await _machineRepo.GetByIdAsync(request.Dto.MachineId);
                    if (machine == null)
                    {
                        throw new ValidationException($"Machine {request.Dto.MachineId} not found");
                    }

                    // Generate washing ID
                    var washingId = await _washingRepo.GetNextWashingIdAsync();

                    // Create washing entity
                    var washing = new Domain.Entities.Washing
                    {
                        WashingId = washingId,
                        MachineId = request.Dto.MachineId,
                        StartUserId = request.Dto.StartUserId,
                        StartDate = DateTime.UtcNow,
                        Status = 'P',
                        StartObservation = request.Dto.StartObservation
                    };

                    // Add washing
                    await _washingRepo.AddAsync(washing);

                    // Create and add prots
                    var prots = request.Dto.ProtEntries.Select(p => new Prot
                    {
                        WashingId = washingId,
                        ProtId = p.ProtId,
                        BatchNumber = p.BatchNumber,
                        BagNumber = p.BagNumber
                    });

                    await _protRepo.AddRangeAsync(prots);

                    // Create response
                    var response = new WashingResponseDto
                    {
                        WashingId = washing.WashingId,
                        MachineId = washing.MachineId,
                        MachineName = machine.Name,
                        StartUserId = washing.StartUserId,
                        StartUserName = user.UserName,
                        StartDate = washing.StartDate,
                        Status = washing.Status.ToString(),
                        StartObservation = washing.StartObservation,
                        Prots = _mapper.Map<List<ProtDto>>(prots)
                    };

                    _logger.LogInformation("\u2705 {Function} [Thread:{ThreadId}] - COMPLETED. Output: {@Response}",
                        function, threadId, response);

                    return response;
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning(ex, "\u26a0\ufe0f {Function} [Thread:{ThreadId}] - VALIDATION FAILED. Input: {@Request}",
                        function, threadId, request.Dto);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "\u274c {Function} [Thread:{ThreadId}] - ERROR. Input: {@Request}",
                        function, threadId, request.Dto);
                    throw;
                }
            }
        }
    }
}
