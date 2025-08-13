using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Controlmat.Application.Common.Queries.User;
using Controlmat.Application.Common.Queries.Machine;
using Controlmat.Application.Common.Dto;

namespace Controlmat.Api.Controllers
{
    /// <summary>
    /// User and Machine Lookup API
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    [Produces("application/json")]
    [Tags("Lookup")]
    public class LookupController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LookupController> _logger;

        public LookupController(IMediator mediator, ILogger<LookupController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all users for dropdown selection
        /// </summary>
        /// <returns>List of users</returns>
        /// <response code="200">Users retrieved successfully</response>
        [HttpGet("users")]
        [ProducesResponseType(typeof(List<UserDto>), 200)]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation("📝 GET /api/lookup/users");
            var result = await _mediator.Send(new GetUsersQuery.Request());
            return Ok(result);
        }

        /// <summary>
        /// Get all machines with availability status
        /// </summary>
        /// <returns>List of machines with availability flags</returns>
        /// <response code="200">Machines retrieved successfully</response>
        [HttpGet("machines")]
        [ProducesResponseType(typeof(List<MachineDto>), 200)]
        public async Task<IActionResult> GetMachines()
        {
            _logger.LogInformation("📝 GET /api/lookup/machines");
            var result = await _mediator.Send(new GetMachinesQuery.Request());
            return Ok(result);
        }
    }
}
