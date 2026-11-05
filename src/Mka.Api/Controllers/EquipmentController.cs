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
    public ActionResult<IEnumerable<EquipmentResponse>> GetAll() => Ok(
        repository.GetAll().Select(e => e.ToResponse())
        );

    [HttpGet("{id:int}")]
    public ActionResult<EquipmentResponse> GetById(int id)
    {
        var equipment = repository.GetById(id);
        if(equipment is null) return NotFound();
        return Ok(equipment.ToResponse());
    }

     [HttpPost]
    public ActionResult<EquipmentResponse> Create(CreateEquipmentRequest equipment)
    {
        var created = repository.Add(equipment.ToEntity());
        var response = created.ToResponse();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }
}