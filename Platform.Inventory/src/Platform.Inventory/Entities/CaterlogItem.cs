using Platform.Code;

namespace Platform.Inventory.Entities;

public class CaterlogItem : IEntity
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }


}
