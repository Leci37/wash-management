using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Controlmat.Application.Common.Queries.Machine;

namespace Controlmat.Api.Controllers;

[ApiController]
[Route("api/machines")]
[Authorize(Roles = "WarehouseUser")]
public class MachinesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MachinesController(IMediator mediator) => _mediator = mediator;

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveWashes()
        => Ok(await _mediator.Send(new GetActiveWashesQuery.Request()));
}

