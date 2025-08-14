using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Controlmat.Application.Common.Constants;
using Controlmat.Application.Common.Dto;
using Controlmat.Domain.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using Controlmat.Application.Common.Exceptions;

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
                _logger.LogInformation("Finishing wash {WashingId} by user {EndUserId}", request.WashingId, request.Dto.EndUserId);

                var washingIdStr = request.WashingId.ToString();
                if (!Regex.IsMatch(washingIdStr, @"^\d{8}$") ||
                    !DateTime.TryParseExact(washingIdStr.Substring(0, 6), "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    throw new ValidationException(ValidationErrorMessages.Washing.InvalidIdFormat(request.WashingId));
                }

                var washing = await _washingRepo.GetByIdAsync(request.WashingId);
                if (washing == null)
                {
                    throw new NotFoundException(ValidationErrorMessages.Washing.NotFound(request.WashingId));
                }

                if (washing.Status == 'F')
                {
                    _logger.LogWarning("⚠️ Washing already finished: {WashingId}", request.WashingId);
                    throw new ConflictException(ValidationErrorMessages.Washing.AlreadyFinished(request.WashingId));
                }

                if (washing.Status != 'P')
                {
                    _logger.LogWarning("⚠️ Invalid status transition from '{CurrentStatus}' to 'F' for washing: {WashingId}",
                        washing.Status, request.WashingId);
                    throw new ConflictException(ValidationErrorMessages.Washing.InvalidStatusTransition(washing.Status, 'F'));
                }

                if (!await _userRepo.ExistsAsync(request.Dto.EndUserId))
                {
                    throw new ValidationException(ValidationErrorMessages.User.EndUserNotFound(request.Dto.EndUserId));
                }

                if (await _photoRepo.CountByWashingIdAsync(request.WashingId) < 1)
                {
                    throw new ValidationException(ValidationErrorMessages.Washing.MustHavePhotosToFinish(request.WashingId));
                }

                if (!string.IsNullOrEmpty(request.Dto.FinishObservation) && request.Dto.FinishObservation.Length > 100)
                {
                    throw new ValidationException(ValidationErrorMessages.Observation.FinishObservationTooLong(100));
                }

                washing.EndUserId = request.Dto.EndUserId;
                washing.FinishObservation = request.Dto.FinishObservation;
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
    }
}
