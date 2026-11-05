using Mka.Api.Models;

namespace Mka.Api.Repositories;


public class InMemoryWorkOrderRepository : IWorkOrderRepository
{
    private readonly List<WorkOrder> _items = [];
    private int _nextId = 1;
    public WorkOrder Add(WorkOrder workOrder)
    {
        workOrder.Id = _nextId++;
        workOrder.Status = WorkOrderStatus.Open;
        workOrder.CreatedAt = DateTime.UtcNow;
        _items.Add(workOrder);
        return workOrder;
    }

    public IReadOnlyList<WorkOrder> GetAll()
    {
        return _items;
    }

    public WorkOrder? GetById(int id)
    {
        return _items.FirstOrDefault(e => e.Id == id);
    }
}