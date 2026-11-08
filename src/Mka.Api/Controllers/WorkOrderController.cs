using Microsoft.AspNetCore.Mvc;
using Mka.Api.Models;
using Mka.Api.Repositories;
using Mka.Api.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Mka.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class WorkOrderController(IWorkOrderRepository workOrderRepository, IEquipmentRepository equipmentRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkOrderResponse>>> GetAll(
    [FromQuery] int? equipmentId, CancellationToken ct)
    {
        var items = await workOrderRepository.GetAllAsync(equipmentId, ct);
        return Ok(items.Select(w => w.ToResponse()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkOrderResponse>> GetById(int id, CancellationToken ct)
    {
        var workOrder = await workOrderRepository.GetByIdAsync(id, ct);
        if (workOrder is null) return NotFound();
        return Ok(workOrder.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<WorkOrderResponse>> Create(
        CreateWorkOrderRequest request, CancellationToken ct)
    {
        var equipment = await equipmentRepository.GetByIdAsync(request.EquipmentId, ct);
        if (equipment is null) return BadRequest("Equipment not found");

        var created = await workOrderRepository.AddAsync(request.ToEntity(), ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToResponse());
    }
}