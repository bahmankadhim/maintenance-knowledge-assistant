using System.ComponentModel.DataAnnotations;
using Mka.Api.Models;

namespace Mka.Api.Dtos;

public record EquipmentResponse(
    int Id, string Tag, string Name, string Type,
    string Location, EquipmentStatus Status, DateOnly InstalledOn);

public record CreateEquipmentRequest(
    [Required, StringLength(50)]
    [RegularExpression(@"^[A-Z]{1,3}-\d{3,4}$",
        ErrorMessage = "Tag must look like P-101 or HX-2001.")]
    string Tag,

    [Required, StringLength(200)]
    string Name,

    [StringLength(100)]
    string Type,

    [StringLength(100)]
    string Location,

    [EnumDataType(typeof(EquipmentStatus))]
    EquipmentStatus Status,

    DateOnly InstalledOn);
