using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Controlmat.Application.Common.Commands.WashCycle;
using Controlmat.Application.Common.Queries.Washing;
using Controlmat.Application.Common.Queries.User;
using Controlmat.Application.Common.Queries.Machine;
using Controlmat.Application.Common.Dto;
using System.ComponentModel.DataAnnotations;

namespace Controlmat.Api.Controllers
{
    /// <summary>
    /// SUMISAN Wash Management API - Surgical Instrument Cleaning Cycles
    /// </summary>
    [ApiController]
    [Route("api/washing")]
    [AllowAnonymous]
    [Produces("application/json")]
    [Tags("Washing")]
    public class WashingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<WashingController> _logger;

        public WashingController(IMediator mediator, ILogger<WashingController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Start a new wash cycle
        /// </summary>
        /// <param name="dto">Wash details including machine, operator, and PROTs</param>
        /// <returns>Created wash details</returns>
        /// <response code="201">Wash started successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="409">Conflict - business rule violation (max 2 active washes, machine in use, etc.)</response>
        [HttpPost]
        [ProducesResponseType(typeof(WashingResponseDto), 201)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        public async Task<IActionResult> StartWash([FromBody] NewWashDto dto)
        {
            _logger.LogInformation("📝 POST /api/washing - Starting new wash for MachineId: {MachineId}, StartUserId: {StartUserId}, ProtCount: {ProtCount}",
                dto.MachineId, dto.StartUserId, dto.ProtEntries?.Count ?? 0);

            try
            {
                var result = await _mediator.Send(new StartWashCommand.Request { Dto = dto });
                return CreatedAtAction(nameof(GetWashById), new { id = result.WashingId }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("⚠️ Business rule violation: {Message}", ex.Message);
                return Conflict(new ProblemDetails 
                { 
                    Title = "Business Rule Violation",
                    Detail = ex.Message,
                    Status = 409
                });
            }
        }

        [HttpPost("lavado")]
        [ProducesResponseType(typeof(WashingResponseDto), 200)]
        public async Task<IActionResult> StartLavado([FromBody] LavadoDto lavado)
        {
            _logger.LogInformation("📝 POST /api/washing/lavado - MachineId: {MachineId}, StartUserId: {StartUserId}, ProtCount: {ProtCount}",
                lavado.MachineId, lavado.StartUserId, lavado.Prots?.Count ?? 0);

            var newWashDto = new NewWashDto
            {
                MachineId = lavado.MachineId,
                StartUserId = lavado.StartUserId,
                StartObservation = lavado.StartObservation,
                ProtEntries = lavado.Prots
            };

            var result = await _mediator.Send(new StartWashCommand.Request { Dto = newWashDto });
            return Ok(result);
        }

        /// <summary>
        /// Finish an active wash cycle
        /// </summary>
        /// <param name="id">Washing ID</param>
        /// <param name="dto">Finish details including end operator and observations</param>
        /// <returns>Updated wash details</returns>
        /// <response code="200">Wash finished successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="403">Forbidden - photo requirements not met</response>
        /// <response code="404">Wash not found</response>
        /// <response code="409">Wash already finished</response>
        [HttpPut("{id}/finish")]
        [ProducesResponseType(typeof(WashingResponseDto), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        public async Task<IActionResult> FinishWash([Required] long id, [FromBody] FinishWashDto dto)
        {
            _logger.LogInformation("📝 PUT /api/washing/{WashingId}/finish - EndUserId: {EndUserId}", id, dto.EndUserId);

            try
            {
                var result = await _mediator.Send(new FinishWashCommand.Request { WashingId = id, Dto = dto });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("⚠️ Business rule violation: {Message}", ex.Message);
                
                if (ex.Message.Contains("not found"))
                    return NotFound(new ProblemDetails { Title = "Wash Not Found", Detail = ex.Message, Status = 404 });
                    
                if (ex.Message.Contains("photo"))
                    return StatusCode(403, new ProblemDetails { Title = "Photos Required", Detail = ex.Message, Status = 403 });
                    
                return Conflict(new ProblemDetails { Title = "Business Rule Violation", Detail = ex.Message, Status = 409 });
            }
        }

        /// <summary>
        /// Upload photo evidence for a wash
        /// </summary>
        /// <param name="id">Washing ID</param>
        /// <param name="file">Image file (JPEG/PNG, max 5MB)</param>
        /// <param name="description">Optional photo description</param>
        /// <returns>Uploaded filename</returns>
        /// <response code="201">Photo uploaded successfully</response>
        /// <response code="400">Invalid file type, size, or wash not found</response>
        /// <response code="409">Photo limit exceeded (99 max per wash)</response>
        /// <response code="413">File too large</response>
        [HttpPost("{id}/photos")]
        [ProducesResponseType(typeof(string), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        [ProducesResponseType(typeof(ProblemDetails), 413)]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB limit
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPhoto([Required] long id, IFormFile file, string? description = null)
        {
            if (file == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "File Required",
                    Detail = "No file was uploaded",
                    Status = 400
                });
            }

            _logger.LogInformation("📝 POST /api/washing/{WashingId}/photos - FileName: {FileName}, Size: {Size} bytes",
                id, file.FileName, file.Length);

            try
            {
                var fileName = await _mediator.Send(new UploadPhotoCommand.Request
                {
                    WashingId = id,
                    File = file,
                    Description = description
                });

                return CreatedAtAction(nameof(GetWashById), new { id }, fileName);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("⚠️ File validation error: {Message}", ex.Message);
                return BadRequest(new ProblemDetails { Title = "Invalid File", Detail = ex.Message, Status = 400 });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("⚠️ Business rule violation: {Message}", ex.Message);

                if (ex.Message.Contains("not found"))
                    return NotFound(new ProblemDetails { Title = "Wash Not Found", Detail = ex.Message, Status = 404 });

                if (ex.Message.Contains("99 photos"))
                    return Conflict(new ProblemDetails { Title = "Photo Limit Exceeded", Detail = ex.Message, Status = 409 });

                return BadRequest(new ProblemDetails { Title = "Upload Error", Detail = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Add a PROT (instrument kit) to an active wash
        /// </summary>
        /// <param name="id">Washing ID</param>
        /// <param name="dto">PROT details (ProtId, BatchNumber, BagNumber)</param>
        /// <returns>Success confirmation</returns>
        /// <response code="200">PROT added successfully</response>
        /// <response code="400">Invalid PROT format</response>
        /// <response code="404">Wash not found</response>
        /// <response code="409">Wash already finished or duplicate PROT</response>
        [HttpPost("{id}/prots")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        public async Task<IActionResult> AddProt([Required] long id, [FromBody] AddProtDto dto)
        {
            _logger.LogInformation("📝 POST /api/washing/{WashingId}/prots - ProtId: {ProtId}, BatchNumber: {BatchNumber}, BagNumber: {BagNumber}", 
                id, dto.ProtId, dto.BatchNumber, dto.BagNumber);

            // Override WashingId from route
            dto.WashingId = id;

            try
            {
                await _mediator.Send(new AddProtCommand.Request { WashingId = id, Dto = dto });
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("⚠️ Business rule violation: {Message}", ex.Message);
                
                if (ex.Message.Contains("not found"))
                    return NotFound(new ProblemDetails { Title = "Wash Not Found", Detail = ex.Message, Status = 404 });
                    
                if (ex.Message.Contains("finished"))
                    return Conflict(new ProblemDetails { Title = "Wash Already Finished", Detail = ex.Message, Status = 409 });
                    
                if (ex.Message.Contains("already exists"))
                    return Conflict(new ProblemDetails { Title = "Duplicate PROT", Detail = ex.Message, Status = 409 });
                    
                return BadRequest(new ProblemDetails { Title = "Add PROT Error", Detail = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Get all active wash cycles (max 2)
        /// </summary>
        /// <returns>List of active washes with basic details</returns>
        /// <response code="200">Success - returns 0-2 active washes</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(List<ActiveWashDto>), 200)]
        public async Task<IActionResult> GetActiveWashes()
        {
            _logger.LogInformation("📝 GET /api/washing/active");

            var result = await _mediator.Send(new GetActiveWashesQuery.Request());
            return Ok(result);
        }

        /// <summary>
        /// Get detailed wash information by ID
        /// </summary>
        /// <param name="id">Washing ID</param>
        /// <returns>Complete wash details including PROTs and photos</returns>
        /// <response code="200">Wash found and returned</response>
        /// <response code="404">Wash not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WashingResponseDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<IActionResult> GetWashById([Required] long id)
        {
            _logger.LogInformation("📝 GET /api/washing/{WashingId}", id);

            var result = await _mediator.Send(new GetWashByIdQuery.Request { WashingId = id });
            
            if (result == null)
            {
                _logger.LogWarning("⚠️ Wash not found: {WashingId}", id);
                return NotFound(new ProblemDetails 
                { 
                    Title = "Wash Not Found", 
                    Detail = $"Washing with ID {id} was not found",
                    Status = 404 
                });
            }

            return Ok(result);
        }

        [HttpGet("{washId}/photos")]
        public async Task<IActionResult> GetWashPhotos(long washId)
            => Ok(await _mediator.Send(new GetWashPhotosQuery.Request(washId)));
    }
}

