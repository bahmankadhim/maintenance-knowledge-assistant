using Mka.Api.Models;

namespace Mka.Api.Repositories;

public class InMemoryEquipmentRepository : IEquipmentRepository
{
    private readonly List<Equipment> _items = [
         new() { Id = 1, Tag = "P-101", Name = "Crude Transfer Pump", Type = "Pump",
                Location = "CPF", Status = EquipmentStatus.Operational,
                InstalledOn = new DateOnly(2019, 5, 12) },
        new() { Id = 2, Tag = "C-201", Name = "Gas Compressor", Type = "Compressor",
                Location = "CPF", Status = EquipmentStatus.UnderMaintenance,
                InstalledOn = new DateOnly(2020, 3, 1) }
    ];
     private int _nextId = 3;

    public Equipment Add(Equipment equipment)
    {
        equipment.Id = _nextId++;
        _items.Add(equipment);
        return equipment;
    }

    public IReadOnlyList<Equipment> GetAll() => _items;

    public Equipment? GetById(int id) => _items.FirstOrDefault(e => e.Id == id);
}