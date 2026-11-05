namespace Mka.Api.Models;

public class WorkOrder
{
    public int Id {get;set;}
    public string Title {get;set;} = string.Empty;
    public string Description {get;set;} = string.Empty;
    public int EquipmentId {get;set;}
    public WorkOrderPriority Priority {get;set;} 
    public WorkOrderStatus Status {get;set;}
    public string? AssignedTo {get;set;}
    public DateTime CreatedAt {get;set;}
}