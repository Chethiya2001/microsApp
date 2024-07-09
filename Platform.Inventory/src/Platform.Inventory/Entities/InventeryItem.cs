using Platform.Code;

namespace Platform.Inventory.Entities;

public class InventeryItem : IEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid CaterlogItemId { get; set; }

    public int Quantity { get; set; }

    public DateTimeOffset AquiredDate { get; set; }

}
