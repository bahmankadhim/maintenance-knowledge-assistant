using Microsoft.EntityFrameworkCore;
using Mka.Api.Data;
using Mka.Api.Models;

namespace Mka.Api.Repositories;

public class EfEquipmentRepository(AppDbContext db) : IEquipmentRepository
{
    public async Task<IReadOnlyList<Equipment>> GetAllAsync(CancellationToken ct = default) =>
        await db.Equipment.AsNoTracking().OrderBy(e => e.Tag).ToListAsync(ct);

    public Task<Equipment?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Equipment.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<Equipment> AddAsync(Equipment equipment, CancellationToken ct = default)
    {
        db.Equipment.Add(equipment);
        await db.SaveChangesAsync(ct);
        return equipment;
    }
}