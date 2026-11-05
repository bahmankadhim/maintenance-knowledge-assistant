using Microsoft.AspNetCore.Mvc;
using Mka.Api.Models;
using Mka.Api.Repositories;

namespace Mka.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController(IEquipmentRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Equipment>> GetAll() => Ok(repository.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<Equipment> GetById(int id)
    {
        var equipment = repository.GetById(id);
        if(equipment is null) return NotFound();
        return Ok(equipment);
    }

     [HttpPost]
    public ActionResult<Equipment> Create(Equipment equipment)
    {
        var created = repository.Add(equipment);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}