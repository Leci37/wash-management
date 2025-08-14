using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Constants;
using Controlmat.Application.Common.Dto;
using Controlmat.Application.Common.Exceptions;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

using System;
using WashingEntity = Controlmat.Domain.Entities.Washing;

namespace Controlmat.Application.Common.Commands.WashCycle;

public static class StartWashCommand
{
    public class Request : IRequest<WashingResponseDto>
    {
        public NewWashDto Dto { get; set; } = default!;
    }

    public class Handler : IRequestHandler<Request, WashingResponseDto>
    {
        private readonly IWashingRepository _washingRepo;

        private readonly IMachineRepository _machineRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        private static readonly Regex ProtIdRegex = new("^PROT[0-9]{3}$", RegexOptions.Compiled);
        private static readonly Regex BatchNumberRegex = new("^NL[0-9]{2}$", RegexOptions.Compiled);
        private static readonly Regex BagNumberRegex = new("^[0-9]{2}/[0-9]{2}$", RegexOptions.Compiled);

        public Handler(
            IWashingRepository washingRepo,
            IMachineRepository machineRepo,
            IUserRepository userRepo,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _washingRepo = washingRepo;
            _machineRepo = machineRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<WashingResponseDto> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var dto = request.Dto;
                _logger.LogInformation("Starting wash: MachineId={MachineId}, StartUserId={StartUserId}", dto.MachineId, dto.StartUserId);


                if (!await _userRepo.ExistsAsync(dto.StartUserId))
                {
                    throw new ValidationException(ValidationErrorMessages.User.StartUserNotFound(dto.StartUserId));
                }

                if (!await _machineRepo.ExistsAsync(dto.MachineId))
                {
                    throw new ValidationException(ValidationErrorMessages.Machine.NotFound(dto.MachineId));
                }

                // Validate maximum concurrent washes before starting new wash
                var activeWashCount = await _washingRepo.CountActiveAsync();
                if (activeWashCount >= 2)
                {
                    _logger.LogWarning("⚠️ Maximum concurrent washes reached: {ActiveCount}/2", activeWashCount);
                    throw new ConflictException(ValidationErrorMessages.Washing.MaxActiveWashesReached);
                }

                // Validate machine is not already in use
                var isMachineInUse = await _washingRepo.IsMachineInUseAsync(dto.MachineId);
                if (isMachineInUse)
                {
                    _logger.LogWarning("⚠️ Machine already in use: {MachineId}", dto.MachineId);
                    throw new ConflictException(ValidationErrorMessages.Machine.AlreadyInUse(dto.MachineId));
                }

                // Double-check concurrent limit with machine-specific validation
                var activeMachineWashes = await _washingRepo.GetActiveWashesByMachineAsync(dto.MachineId);
                if (activeMachineWashes.Any())
                {
                    _logger.LogWarning("⚠️ Machine has active wash: {MachineId}", dto.MachineId);
                    throw new ConflictException(ValidationErrorMessages.BusinessRules.OnlyOneWashPerMachine);
                }

                if (dto.ProtEntries == null || !dto.ProtEntries.Any())
                {
                    throw new ValidationException(ValidationErrorMessages.Washing.MustHaveProtsToStart);
                }

                var protKeys = new HashSet<string>();
                foreach (var p in dto.ProtEntries)
                {
                    if (!ProtIdRegex.IsMatch(p.ProtId))
                    {
                        throw new ValidationException(ValidationErrorMessages.Prot.InvalidProtIdFormat(p.ProtId));
                    }

                    if (!BatchNumberRegex.IsMatch(p.BatchNumber))
                    {
                        throw new ValidationException(ValidationErrorMessages.Prot.InvalidBatchNumberFormat(p.BatchNumber));
                    }

                    if (!BagNumberRegex.IsMatch(p.BagNumber))
                    {
                        throw new ValidationException(ValidationErrorMessages.Prot.InvalidBagNumberFormat(p.BagNumber));
                    }

                    var key = $"{p.ProtId}-{p.BatchNumber}-{p.BagNumber}";
                    if (!protKeys.Add(key))
                    {
                        throw new ValidationException(ValidationErrorMessages.Prot.DuplicateProtInRequest);
                    }
                }

                if (!string.IsNullOrEmpty(dto.StartObservation) && dto.StartObservation.Length > 100)
                {
                    throw new ValidationException(ValidationErrorMessages.Observation.StartObservationTooLong(100));
                }

                // Generate proper WashingId in YYMMDDXX format
                var washingId = await GenerateWashingIdAsync();
                _logger.LogInformation("📋 Generated WashingId: {WashingId}", washingId);

                var washing = new WashingEntity
                {
                    WashingId = washingId,
                    MachineId = dto.MachineId,
                    StartUserId = dto.StartUserId,
                    StartDate = DateTime.UtcNow,
                    Status = 'P',
                    StartObservation = dto.StartObservation,
                    Prots = dto.ProtEntries.Select(p => new Prot
                    {
                        ProtId = p.ProtId,
                        BatchNumber = p.BatchNumber,
                        BagNumber = p.BagNumber
                    }).ToList()
                };

                await _washingRepo.AddAsync(washing);
                _logger.LogInformation("Wash {WashingId} started successfully", washing.WashingId);

                return _mapper.Map<WashingResponseDto>(washing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting wash");
                throw;
            }
        }


        private async Task<long> GenerateWashingIdAsync()
        {
            var today = DateTime.UtcNow.Date;
            var prefix = today.ToString("yyMMdd");
            var maxId = await _washingRepo.GetMaxWashingIdByDateAsync(today);
            var sequence = (maxId.HasValue ? (int)(maxId.Value % 100) : 0) + 1;
            return long.Parse($"{prefix}{sequence:D2}");
        }
    }
}
