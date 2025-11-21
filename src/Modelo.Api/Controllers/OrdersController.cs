using MediatR;
using Microsoft.AspNetCore.Mvc;
using Modelo.Application.Commands.Orders;

namespace Modelo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        // Como estamos usando repositório em memória, este endpoint é apenas ilustrativo
        return Ok(new { id });
    }
}
