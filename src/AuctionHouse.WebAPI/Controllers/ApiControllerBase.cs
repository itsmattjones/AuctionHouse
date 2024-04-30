namespace AuctionHouse.WebAPI.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[ApiController]
[Route("api/[controller]")]
public class ApiControllerBase : ControllerBase
{
    private IMediator _mediator;

    protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());
}