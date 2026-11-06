using Mka.Api.Models;

namespace Mka.Api.Repositories;

public interface IWorkOrderRepository
{
    Task<IReadOnlyList<WorkOrder>> GetAllAsync(int? equipmentId = null, CancellationToken ct = default);
    Task<WorkOrder?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<WorkOrder> AddAsync(WorkOrder workOrder, CancellationToken ct = default);
}