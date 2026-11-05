using Mka.Api.Models;

namespace Mka.Api.Repositories;

public interface IEquipmentRepository
{
    IReadOnlyList<Equipment> GetAll();
    Equipment? GetById(int id);
    Equipment Add(Equipment equipment);
}