using Microsoft.AspNetCore.Mvc;
using Mka.Api.Models;
using Mka.Api.Repositories;
using Mka.Api.Dtos;

namespace Mka.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController(IEquipmentRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EquipmentResponse>>> GetAll(CancellationToken ct)
    {
        var items = await repository.GetAllAsync(ct);
        return Ok(items.Select(e => e.ToResponse()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EquipmentResponse>> GetById(int id, CancellationToken ct)
    {
        var equipment = await repository.GetByIdAsync(id, ct);
        if (equipment is null) return NotFound();
        return Ok(equipment.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<EquipmentResponse>> Create(CreateEquipmentRequest equipment, CancellationToken ct)
    {
        var created = await repository.AddAsync(equipment.ToEntity(), ct);
        var response = created.ToResponse();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }
}