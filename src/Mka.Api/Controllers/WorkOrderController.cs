using Microsoft.AspNetCore.Mvc;
using Mka.Api.Models;
using Mka.Api.Repositories;
using Mka.Api.Dtos;

namespace Mka.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkOrderController(IWorkOrderRepository workOrderRepository, IEquipmentRepository equipmentRepository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<WorkOrderResponse>> GetAll([FromQuery] int? equipmentId)
    {
        var items = workOrderRepository.GetAll().AsEnumerable();
        if (equipmentId is not null)
            items = items.Where(w => w.EquipmentId == equipmentId);
        return Ok(items.Select(w => w.ToResponse()));
    }

    [HttpGet("{id:int}")]
    public ActionResult<WorkOrderResponse> GetById(int id)
    {
        var workOrder = workOrderRepository.GetById(id);
        if(workOrder is null) return NotFound();
        return Ok(workOrder.ToResponse());
    }


    [HttpPost]
    public ActionResult<WorkOrderResponse> Create(CreateWorkOrderRequest workOrder)
    {
        var exist = equipmentRepository.GetById(workOrder.EquipmentId);
        if(exist is null) return BadRequest("Equipment not found");

        var created = workOrderRepository.Add(workOrder.ToEntity());
        var response = created.ToResponse();
        return CreatedAtAction(nameof(GetById), new {id = created.Id}, response);
    }
}