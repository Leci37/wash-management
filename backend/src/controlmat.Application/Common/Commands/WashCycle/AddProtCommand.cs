
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Controlmat.Application.Common.Constants;
using Controlmat.Application.Common.Dto;
using Controlmat.Application.Common.Exceptions;
using Controlmat.Domain.Entities;
using Controlmat.Domain.Interfaces;


namespace Controlmat.Application.Common.Commands.WashCycle;

public static class AddProtCommand
{

    public class Request : IRequest<ProtDto>
    {
        public long WashingId { get; set; }
        public ProtDto Dto { get; set; } = default!;
    }

    public class Handler : IRequestHandler<Request, ProtDto>
    {
        private readonly IWashingRepository _washingRepo;
        private readonly IProtRepository _protRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(IWashingRepository washingRepo, IProtRepository protRepo, IMapper mapper, ILogger<Handler> logger)
        {
            _washingRepo = washingRepo;
            _protRepo = protRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProtDto> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var washingId = request.WashingId;
                var dto = request.Dto;

                _logger.LogInformation("Adding PROT {ProtId} to wash {WashingId}", dto.ProtId, washingId);

                if (!IsValidWashingId(washingId))
                    throw new ValidationException(ValidationErrorMessages.Washing.InvalidIdFormat(washingId));

                var washing = await _washingRepo.GetByIdAsync(washingId);
                if (washing == null)
                    throw new ValidationException(ValidationErrorMessages.Washing.NotFound(washingId));

                if (washing.Status != 'P')
                {
                    _logger.LogWarning("⚠️ Cannot modify washing with status '{Status}': {WashingId}",
                        washing.Status, washingId);
                    throw new ConflictException(ValidationErrorMessages.Washing.CannotModifyFinished(washingId));
                }

                if (!Regex.IsMatch(dto.ProtId, @"^PROT[0-9]{3}$"))
                    throw new ValidationException(ValidationErrorMessages.Prot.InvalidProtIdFormat(dto.ProtId));

                if (!Regex.IsMatch(dto.BatchNumber, @"^NL[0-9]{2}$"))
                    throw new ValidationException(ValidationErrorMessages.Prot.InvalidBatchNumberFormat(dto.BatchNumber));

                if (!Regex.IsMatch(dto.BagNumber, @"^[0-9]{2}/[0-9]{2}$"))
                    throw new ValidationException(ValidationErrorMessages.Prot.InvalidBagNumberFormat(dto.BagNumber));

                if (await _protRepo.ExistsInWashAsync(washingId, dto.ProtId, dto.BatchNumber, dto.BagNumber))
                    throw new ConflictException(ValidationErrorMessages.Prot.DuplicateProtInWash(dto.ProtId, dto.BatchNumber, dto.BagNumber));

                var prot = _mapper.Map<Prot>(dto);
                prot.WashingId = washingId;

                await _protRepo.AddAsync(prot);

                return _mapper.Map<ProtDto>(prot);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding PROT to wash {WashingId}", request.WashingId);
                throw;
            }
        }

        private static bool IsValidWashingId(long washingId)
        {
            var idStr = washingId.ToString();
            if (!Regex.IsMatch(idStr, @"^\d{8}$"))
                return false;
            return DateTime.TryParseExact(idStr.Substring(0, 6), "yyMMdd", null, System.Globalization.DateTimeStyles.None, out _);
        }

    }
}
