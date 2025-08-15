using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Controlmat.Domain.Interfaces;
using Controlmat.Domain.Repositories;
using Controlmat.Domain.Entities;
using Controlmat.Application.Common.Constants;
using Controlmat.Application.Common.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Controlmat.Application.Common.Commands.WashCycle;

public static class UploadPhotoCommand
{
    public class Request : IRequest<string>
    {
        public long WashingId { get; set; }
        public IFormFile File { get; set; } = default!;
        public string? Description { get; set; }
    }

    public class Handler : IRequestHandler<Request, string>
    {
        private readonly IWashingRepository _washingRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly IParameterRepository _parameterRepository;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IWashingRepository washingRepository,
            IPhotoRepository photoRepository,
            IParameterRepository parameterRepository,
            ILogger<Handler> logger)
        {
            _washingRepository = washingRepository;
            _photoRepository = photoRepository;
            _parameterRepository = parameterRepository;
            _logger = logger;
        }

        public async Task<string> Handle(Request request, CancellationToken cancellationToken)
        {
            var function = nameof(Handle);
            _logger.LogInformation("🌀 {Function} - Started photo upload for wash: {WashingId}", function, request.WashingId);

            try
            {
                var imagePath = await _parameterRepository.GetImagePathAsync();
                var maxPhotos = await _parameterRepository.GetMaxPhotosPerWashAsync();
                var supportedTypes = await _parameterRepository.GetSupportedFileTypesAsync();

                var washing = await _washingRepository.GetByIdAsync(request.WashingId);
                if (washing == null)
                    throw new NotFoundException(ValidationErrorMessages.Washing.NotFound(request.WashingId));

                if (washing.Status != 'P')
                    throw new ConflictException(ValidationErrorMessages.Washing.AlreadyFinished(request.WashingId));

                var currentPhotoCount = await _photoRepository.CountByWashingIdAsync(request.WashingId);
                if (currentPhotoCount >= maxPhotos)
                    throw new ConflictException(ValidationErrorMessages.Photo.MaxPhotosReached(request.WashingId, maxPhotos));

                var fileExtension = Path.GetExtension(request.File.FileName)?.TrimStart('.').ToLower();
                if (!supportedTypes.Contains(fileExtension))
                    throw new ValidationException(ValidationErrorMessages.Photo.InvalidFileType(request.File.ContentType));

                var sequenceNumber = (currentPhotoCount + 1).ToString("D2");
                var fileName = $"{request.WashingId}_{sequenceNumber}.jpg";

                var currentYear = DateTime.Now.Year.ToString();
                var fullDirectoryPath = Path.Combine(imagePath, currentYear);

                if (!Directory.Exists(fullDirectoryPath))
                {
                    Directory.CreateDirectory(fullDirectoryPath);
                    _logger.LogInformation("📁 Created directory: {DirectoryPath}", fullDirectoryPath);
                }

                var fullFilePath = Path.Combine(fullDirectoryPath, fileName);

                using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream, cancellationToken);
                }

                var photo = new Photo
                {
                    WashingId = request.WashingId,
                    FileName = fileName,
                    FilePath = fullFilePath,
                    CreatedAt = DateTime.UtcNow
                };

                await _photoRepository.AddAsync(photo);

                _logger.LogInformation("✅ {Function} - Photo saved successfully: {FileName} at {FilePath}",
                    function, fileName, fullFilePath);

                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ {Function} - Failed to upload photo for wash: {WashingId}", function, request.WashingId);
                throw;
            }
        }
    }
}
