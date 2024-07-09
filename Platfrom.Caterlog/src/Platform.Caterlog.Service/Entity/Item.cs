


using Platform.Code;

namespace Platform.Caterlog.Service.Entity;

public class Item : IEntity
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public DateTimeOffset CareatedDate { get; set; }
}
