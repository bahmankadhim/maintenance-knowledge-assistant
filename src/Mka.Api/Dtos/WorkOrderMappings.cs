using Mka.Api.Models;

namespace Mka.Api.Dtos;

public static class WorkOrderMappings
{
    public static WorkOrderResponse ToResponse(this WorkOrder e) => 
        new (e.Id, e.Title, e.Description, e.EquipmentId, e.Priority, e.Status, e.AssignedTo, e.CreatedAt);

    public static WorkOrder ToEntity(this CreateWorkOrderRequest e) => new()
    {
        Title = e.Title,
        Description = e.Description,
        EquipmentId = e.EquipmentId,
        Priority = e.Priority,
        AssignedTo = e.AssignedTo
    };
}