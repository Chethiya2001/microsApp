using Platform.Caterlog.Service.Entity;
using static Play.Caterlog.Service.Dtos;

namespace Platform.Caterlog.Service;

public static class Extentions
{
    public static ItemDto AsDto(this Item item)
    {
        return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CareatedDate);
    }
}
