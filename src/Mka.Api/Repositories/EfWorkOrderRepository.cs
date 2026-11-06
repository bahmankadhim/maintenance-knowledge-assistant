using Microsoft.EntityFrameworkCore;
using Mka.Api.Data;
using Mka.Api.Models;

namespace Mka.Api.Repositories;

public class EfWorkOrderRepository(AppDbContext db) : IWorkOrderRepository
{
    public async Task<WorkOrder> AddAsync(WorkOrder workOrder, CancellationToken ct = default)
    {
        workOrder.Status = WorkOrderStatus.Open;
        workOrder.CreatedAt = DateTime.UtcNow;
        db.WorkOrders.Add(workOrder);
        await db.SaveChangesAsync(ct);
        return workOrder;
    }

    public async Task<IReadOnlyList<WorkOrder>> GetAllAsync(int? equipmentId = null, CancellationToken ct = default)
    {
        var query = db.WorkOrders.AsNoTracking();
        if (equipmentId is not null)
        {
            query = query.Where(o => o.EquipmentId == equipmentId);
        }
        return await query.OrderBy(o => o.CreatedAt).ToListAsync(ct);
    }

    public async Task<WorkOrder?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await db.WorkOrders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id, ct);
}