using Platform.Inventory.Entities;
using static Platform.Inventory.Dtos;

namespace Platform.Inventory;

public static class Extentions
{
    public static InventoryItemsDto AsDto(this InventeryItem item, string name, string discription)
    {
        return new InventoryItemsDto(item.Id, item.Quantity, name, discription, item.AquiredDate);
    }
}
