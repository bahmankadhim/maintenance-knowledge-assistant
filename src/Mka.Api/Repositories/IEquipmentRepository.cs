using Mka.Api.Models;

namespace Mka.Api.Repositories;

public interface IEquipmentRepository
{
    Task<IReadOnlyList<Equipment>> GetAllAsync(CancellationToken ct = default);
    Task<Equipment?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Equipment> AddAsync(Equipment equipment, CancellationToken ct = default);
}