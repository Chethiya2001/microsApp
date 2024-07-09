using MassTransit;
using Platform.Common;
using Platform.Inventory.Entities;
using static Play.Caterlog.Cantracts.Cantracts;

namespace Platform.Inventory.Consumers;

public class CaterlogItemCreatedConsumer : IConsumer<CaterlogItemCreated>
{
    private readonly IRepository<CaterlogItem> repository;

    public CaterlogItemCreatedConsumer(IRepository<CaterlogItem> repository)
    {
        this.repository = repository;
    }
    public async Task Consume(ConsumeContext<CaterlogItemCreated> context)
    {
        var message = context.Message;
        var item = await repository.GetAsync(message.ItemId);

        if (item != null)
        {
            return;
        }

        item = new CaterlogItem
        {
            Id = message.ItemId,
            Name = message.Name,
            Description = message.Description
        };
        await repository.CreateAsync(item);
    }
}
