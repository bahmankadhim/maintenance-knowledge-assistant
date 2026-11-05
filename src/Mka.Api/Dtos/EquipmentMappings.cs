using Mka.Api.Models;

namespace Mka.Api.Dtos;

public static class EquipmentMappings
{
    public static EquipmentResponse ToResponse(this Equipment e) => 
        new (e.Id, e.Tag, e.Name, e.Type, e.Location, e.Status, e.InstalledOn);

    public static Equipment ToEntity(this CreateEquipmentRequest r) => new()
    {
        Tag = r.Tag,
        Name = r.Name,
        Type = r.Type,
        Location = r.Location,
        Status = r.Status,
        InstalledOn = r.InstalledOn
    }
}