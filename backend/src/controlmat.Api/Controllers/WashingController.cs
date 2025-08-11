using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Controlmat.Application.Common.Commands.Washing;
using Controlmat.Application.Common.Queries.Washing;
using Controlmat.Application.Common.Dto;

namespace Controlmat.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "WarehouseUser")]
    public class WashingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WashingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Start([FromBody] NewWashDto dto)
        {
            try
            {
                var result = await _mediator.Send(new StartWashCommand.Request { Dto = dto });
                return CreatedAtAction(nameof(GetById), new { id = result.WashingId }, result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var result = await _mediator.Send(new GetActiveWashesQuery.Request());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            // TODO: Implement GetWashByIdQuery
            return NotFound();
        }
    }
}
