using MassTransit;
using Platform.Common;
using Platform.Inventory.Entities;
using static Play.Caterlog.Cantracts.Cantracts;

namespace Platform.Inventory.Consumers;

public class CaterlogItemDeletedConsumer : IConsumer<CaterlogItemDeleted>
{
    private readonly IRepository<CaterlogItem> repository;

    public CaterlogItemDeletedConsumer(IRepository<CaterlogItem> repository)
    {
        this.repository = repository;
    }

    public async Task Consume(ConsumeContext<CaterlogItemDeleted> context)
    {
        var message = context.Message;
        var item = await repository.GetAsync(message.ItemId);

        if (item == null)
        {
            return;
        }

        await repository.RemoveAsync(item.Id);

    }

}
