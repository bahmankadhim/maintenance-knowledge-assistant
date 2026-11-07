using System.ComponentModel.DataAnnotations;
using Mka.Api.Models;

namespace Mka.Api.Dtos;

public record WorkOrderResponse(
    int Id, string Title, string Description, int EquipmentId, WorkOrderPriority Priority,
    WorkOrderStatus Status, string? AssignedTo, DateTime CreatedAt);

public record CreateWorkOrderRequest(
    [Required, StringLength(200, MinimumLength = 5)]
    string Title,

    [StringLength(2000)]
    string Description,

    [Range(1, int.MaxValue, ErrorMessage = "EquipmentId must be a positive number.")]
    int EquipmentId,

    [EnumDataType(typeof(WorkOrderPriority))]
    WorkOrderPriority Priority,

    [StringLength(100)]
    string? AssignedTo);
