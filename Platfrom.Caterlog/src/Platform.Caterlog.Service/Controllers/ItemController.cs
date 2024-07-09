
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Platform.Caterlog.Service.Entity;
using Platform.Common;
using static Play.Caterlog.Cantracts.Cantracts;

using static Play.Caterlog.Service.Dtos;

namespace Platform.Caterlog.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> _itemsRepository;
    //push messages
    private readonly IPublishEndpoint _publishEndpoint;


    public ItemsController(IRepository<Item> repository, IPublishEndpoint publishEndpoint)
    {
        _itemsRepository = repository;
        _publishEndpoint = publishEndpoint;
    }




    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
    {

        var items = (await _itemsRepository.GetAllAsync())
                    .Select(item => item.AsDto());

        return Ok(items);
    }

    // GET /items/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
    {
        var item = await _itemsRepository.GetAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        return item.AsDto();
    }

    // POST /items
    [HttpPost]
    public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
    {
        var item = new Item
        {
            Name = createItemDto.Name,
            Description = createItemDto.Description,
            Price = createItemDto.Price,
            CareatedDate = DateTimeOffset.UtcNow
        };

        await _itemsRepository.CreateAsync(item);
        //push message
        await _publishEndpoint.Publish(new CaterlogItemCreated(item.Id, item.Name, item.Description));
        return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
    }

    // PUT /items/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateItemmDto updateItemDto)
    {
        var existingItem = await _itemsRepository.GetAsync(id);

        if (existingItem == null)
        {
            return NotFound();
        }

        existingItem.Name = updateItemDto.Name;
        existingItem.Description = updateItemDto.Description;
        existingItem.Price = updateItemDto.Price;

        await _itemsRepository.UpdateAsync(existingItem);
        await _publishEndpoint.Publish(new CaterlogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));



        return NoContent();
    }

    // DELETE /items/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var item = await _itemsRepository.GetAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        await _itemsRepository.RemoveAsync(item.Id);
        await _publishEndpoint.Publish(new CaterlogItemDeleted(item.Id));




        return NoContent();
    }
}
