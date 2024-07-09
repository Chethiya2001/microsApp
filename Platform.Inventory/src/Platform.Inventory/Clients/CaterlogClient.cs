using System.Net.Http;
using static Platform.Inventory.Dtos;

public class CaterlogClient
{
    private readonly HttpClient httpClient;

    public CaterlogClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
    //retrive itesms from caterlog
    public async Task<IReadOnlyCollection<CaterlogItemDto>> GetCaterlogItemsAsync()
    {
        var items = await httpClient.GetFromJsonAsync<IReadOnlyCollection<CaterlogItemDto>>("/items");
        return items!;

    }
}
