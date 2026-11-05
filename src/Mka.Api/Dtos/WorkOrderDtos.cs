using Mka.Api.Models;

namespace Mka.Api.Dtos;

public record WorkOrderResponse(
    int Id, string Title, string Description, int EquipmentId, WorkOrderPriority Priority,
    WorkOrderStatus Status, string? AssignedTo, DateTime CreatedAt
);

public record CreateWorkOrderRequest(
    string Title, string Description, int EquipmentId, WorkOrderPriority Priority,
    string? AssignedTo
);

