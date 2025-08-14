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

                if (await _washingRepo.IsMachineInUseAsync(dto.MachineId))
                {
                    throw new ConflictException(ValidationErrorMessages.Machine.AlreadyInUse(dto.MachineId));
                }

                if (await _washingRepo.CountActiveAsync() >= 2)
                {
                    throw new ConflictException(ValidationErrorMessages.Washing.MaxActiveWashesReached);
                }

                if (dto.ProtEntries == null || !dto.ProtEntries.Any())
                {
                    throw new ValidationException(ValidationErrorMessages.Washing.MustHaveProtsToStart);
                }

                var protKeys = new HashSet<string>();
                foreach (var p in dto.ProtEntries)
                {
                    if (!Regex.IsMatch(p.ProtId, @"^PROT[0-9]{3}$"))
                    {
                        throw new ValidationException(ValidationErrorMessages.Prot.InvalidProtIdFormat(p.ProtId));
                    }

                    if (!Regex.IsMatch(p.BatchNumber, @"^NL[0-9]{2}$"))
                    {
                        throw new ValidationException(ValidationErrorMessages.Prot.InvalidBatchNumberFormat(p.BatchNumber));
                    }

                    if (!Regex.IsMatch(p.BagNumber, @"^[0-9]{2}/[0-9]{2}$"))
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
