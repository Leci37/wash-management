using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Controlmat.Application.Common.Queries.Photo;

namespace Controlmat.Api.Controllers;

[ApiController]
[Route("api/photos")]
[Authorize(Roles = "WarehouseUser")]
public class PhotosController : ControllerBase
{
    private readonly IMediator _mediator;

    public PhotosController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{photoId}/download")]
    public async Task<IActionResult> DownloadPhoto(int photoId)
    {
        var result = await _mediator.Send(new DownloadSinglePhotoQuery.Request(photoId));
        return File(result.FileBytes, result.ContentType, result.FileName);
    }
}
