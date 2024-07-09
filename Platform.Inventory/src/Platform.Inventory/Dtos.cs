namespace Platform.Inventory;

public class Dtos
{
    public record GrantItemDto(Guid UserId, Guid CaterlogItemId, int Quantity);
    public record InventoryItemsDto(Guid CaterlogItemId, int Quantity, string Name, string Description, DateTimeOffset AquiredDate);
    public record CaterlogItemDto(Guid Id, string? Name, string? Description);

}
