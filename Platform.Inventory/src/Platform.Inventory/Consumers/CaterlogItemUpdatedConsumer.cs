using MassTransit;
using Platform.Common;
using Platform.Inventory.Entities;
using static Play.Caterlog.Cantracts.Cantracts;

namespace Platform.Inventory.Consumers;

public class CaterlogItemUpdatedConsumer : IConsumer<CaterlogItemUpdated>
{
    private readonly IRepository<CaterlogItem> repository;

    public CaterlogItemUpdatedConsumer(IRepository<CaterlogItem> repository)
    {
        this.repository = repository;
    }
    public async Task Consume(ConsumeContext<CaterlogItemUpdated> context)
    {
        var message = context.Message;
        var item = await repository.GetAsync(message.ItemId);

        if (item == null)
        {
            item = new CaterlogItem
            {
                Id = message.ItemId,
                Name = message.Name,
                Description = message.Description
            };
            await repository.CreateAsync(item);
            Console.WriteLine($"Created new item with ID {message.ItemId}");
        }
        else
        {
            item.Name = message.Name;
            item.Description = message.Description;

            await repository.UpdateAsync(item);
            Console.WriteLine($"Updated item with ID {message.ItemId}");
        }

    }

}
