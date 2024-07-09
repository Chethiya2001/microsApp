using System.ComponentModel.DataAnnotations;

namespace Play.Caterlog.Service;

public class Dtos
{
    public record ItemDto(Guid Id, string? Name, string? Description, decimal Price, DateTimeOffset CreatedDate);
    public record CreateItemDto([Required] string Name, string Description, [Range(0, 1000)] decimal Price);
    public record UpdateItemmDto([Required] string Name, string Description, [Range(0, 1000)] decimal Price);
}
