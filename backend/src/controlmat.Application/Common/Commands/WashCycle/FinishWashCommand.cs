using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Constants;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;
using System;
using System.Text.RegularExpressions;

namespace Controlmat.Application.Common.Commands.WashCycle;

public static class FinishWashCommand
{
    public class Request : IRequest<WashingResponseDto>
    {
        public long WashingId { get; set; }
        public FinishWashDto Dto { get; set; } = default!;
    }

    public class Handler : IRequestHandler<Request, WashingResponseDto>
    {
        private readonly IWashingRepository _washingRepo;
        private readonly IUserRepository _userRepo;
        private readonly IPhotoRepository _photoRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IWashingRepository washingRepo,
            IUserRepository userRepo,
            IPhotoRepository photoRepo,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _washingRepo = washingRepo;
            _userRepo = userRepo;
            _photoRepo = photoRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<WashingResponseDto> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var washingId = request.WashingId;
                var dto = request.Dto;

                _logger.LogInformation("Finishing wash {WashingId} by user {EndUserId}", washingId, dto.EndUserId);

                if (!IsValidWashingId(washingId))
                    throw new ArgumentException(ValidationErrorMessages.Washing.InvalidIdFormat(washingId));

                var washing = await _washingRepo.GetByIdAsync(washingId)
                    ?? throw new InvalidOperationException(ValidationErrorMessages.Washing.NotFound(washingId));

                if (!await _userRepo.ExistsAsync(dto.EndUserId))
                    throw new InvalidOperationException(ValidationErrorMessages.User.EndUserNotFound(dto.EndUserId));

                if (washing.Status != 'P')
                    throw new InvalidOperationException(ValidationErrorMessages.Washing.NotInProgress(washingId));

                if (await _photoRepo.CountByWashingIdAsync(washingId) < 1)
                    throw new InvalidOperationException(ValidationErrorMessages.Washing.MustHavePhotosToFinish(washingId));

                if (!string.IsNullOrEmpty(dto.FinishObservation) && dto.FinishObservation.Length > 100)
                    throw new ArgumentException(ValidationErrorMessages.Observation.FinishObservationTooLong(100));

                washing.EndUserId = dto.EndUserId;
                washing.FinishObservation = dto.FinishObservation;
                washing.EndDate = DateTime.UtcNow;
                washing.Status = 'F';

                await _washingRepo.UpdateAsync(washing);

                _logger.LogInformation("Wash {WashingId} finished", washing.WashingId);

                return _mapper.Map<WashingResponseDto>(washing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finishing wash {WashingId}", request.WashingId);
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
