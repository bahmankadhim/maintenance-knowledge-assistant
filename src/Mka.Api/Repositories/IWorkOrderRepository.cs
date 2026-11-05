using Mka.Api.Models;

namespace Mka.Api.Repositories;

public interface IWorkOrderRepository
{
    IReadOnlyList<WorkOrder> GetAll();
    WorkOrder? GetById(int id);
    WorkOrder Add(WorkOrder workOrder);
}