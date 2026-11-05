namespace Mka.Api.Models;

public class Equipment
{
    public int Id {get;set;}
    public string Tag {get;set;} = string.Empty;
    public string Name {get;set;} = string.Empty;
    public string Type {get;set;} = string.Empty;
    public string Location {get;set;} = string.Empty;
    public EquipmentStatus Status {get;set;} = EquipmentStatus.Operational;
    public DateOnly InstalledOn {get;set;}
}