namespace Play.Caterlog.Cantracts;

public class Cantracts
{
    public record CaterlogItemCreated(Guid ItemId, string Name, string Description);
    public record CaterlogItemUpdated(Guid ItemId, string Name, string Description);
    public record CaterlogItemDeleted(Guid ItemId);
}
