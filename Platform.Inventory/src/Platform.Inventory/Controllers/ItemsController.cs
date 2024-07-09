using Microsoft.AspNetCore.Mvc;
using Platform.Common;
using Platform.Inventory.Entities;
using static Platform.Inventory.Dtos;

namespace Platform.Inventory.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<InventeryItem> inventoryitemsRepository;
    private readonly IRepository<CaterlogItem> caterlogRepository;
    private readonly ILogger<ItemsController> logger;

    public ItemsController(ILogger<ItemsController> logger, IRepository<InventeryItem> inventoryitemsRepository, IRepository<CaterlogItem> caterlogRepository)
    {
        this.inventoryitemsRepository = inventoryitemsRepository;
        this.caterlogRepository = caterlogRepository;
        this.logger = logger;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemsDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest();
        }
        // var items = (await inventoryitemsRepository.GetAllAsync(item => item.UserId == userId))
        // .Select(item => item.AsDto());


        //1.GetHashCode caterlogitems
        //var caterlogItems = await caterlogClient.GetCaterlogItemsAsync();
        //2.Get currently owned inventory items
        logger.LogInformation("Fetching inventory items for user {UserId}", userId);

        var inventoryItemEntity = await inventoryitemsRepository.GetAllAsync(item => item.UserId == userId);
        if (!inventoryItemEntity.Any())
        {
            logger.LogWarning("No inventory items found for user {UserId}", userId);
            return NotFound();
        }
        //new all crp ct items connect

        var itemIds = inventoryItemEntity.Select(item => item.CaterlogItemId);
        logger.LogInformation("Fetching catalog items for item IDs: {ItemIds}", itemIds);
        //actual c items get
        var caterlogItemsEntities = await caterlogRepository.GetAllAsync(item => itemIds.Contains(item.Id));

        //3.combine
        var inventoryItemDtos = inventoryItemEntity.Select(inventoryItems =>
        {
            var caterlogItem = caterlogItemsEntities.Single(caterlogItem => caterlogItem.Id == inventoryItems.CaterlogItemId);
            return inventoryItems.AsDto(caterlogItem.Name!, caterlogItem.Description!);
        }).Where(dto => dto != null).ToList();

        logger.LogInformation("Returning {Count} inventory items for user {UserId}", inventoryItemDtos.Count, userId);
        return Ok(inventoryItemDtos);

    }
    //assign items
    [HttpPost]
    public async Task<ActionResult> PostAsync(GrantItemDto grantitemDto)
    {
        var inventoryItem = await inventoryitemsRepository.GetAsync(
        item => item.UserId == grantitemDto.UserId &&

        item.CaterlogItemId == grantitemDto.CaterlogItemId
        );

        if (inventoryItem == null)
        {
            inventoryItem = new InventeryItem
            {
                CaterlogItemId = grantitemDto.CaterlogItemId,
                UserId = grantitemDto.UserId,
                Quantity = grantitemDto.Quantity,
                AquiredDate = DateTimeOffset.UtcNow
            };
            await inventoryitemsRepository.CreateAsync(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += grantitemDto.Quantity;
            await inventoryitemsRepository.UpdateAsync(inventoryItem);
        }
        return Ok(inventoryItem);
    }
}
