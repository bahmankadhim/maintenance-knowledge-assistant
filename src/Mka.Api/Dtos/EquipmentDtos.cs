using Mka.Api.Models;

namespace Mka.Api.Dtos;

public record EquipmentResponse(
    int Id, string Tag, string Name, string Type,
    string Location, EquipmentStatus Status, DateOnly InstalledOn);

public record CreateEquipmentRequest(
    string Tag, string Name, string Type,
    string Location, EquipmentStatus Status, DateOnly InstalledOn);

