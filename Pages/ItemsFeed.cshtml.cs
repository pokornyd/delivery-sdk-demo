using System.Diagnostics;
using Kontent.Ai.Delivery.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryDemo.Pages;

public class ItemsFeedModel : PageModel
{
    private readonly IDeliveryClient _client;
    private readonly ILogger<ItemsFeedModel> _logger;

    public ItemsFeedModel(IDeliveryClient client, ILogger<ItemsFeedModel> logger)
    {
        _client = client;
        _logger = logger;
    }

    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public List<IContentItem<IDynamicElements>>? Items { get; set; }
    public int MaxItems { get; set; } = 20;
    public TimeSpan ExecutionTime { get; set; }

    public async Task OnGetAsync(int maxItems = 20)
    {
        MaxItems = maxItems;
        Items = new List<IContentItem<IDynamicElements>>();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var query = _client.GetItemsFeed()
                .Where(f => f.System("type").IsEqualTo("article"))
                .OrderBy("system.last_modified", OrderingMode.Descending);

            await foreach (var item in query.EnumerateItemsAsync())
            {
                Items.Add(item);
                if (Items.Count >= MaxItems)
                {
                    break;
                }
            }

            IsSuccess = true;
            _logger.LogInformation("Successfully retrieved {Count} items using feed", Items.Count);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _logger.LogError(ex, "Exception while retrieving items feed");
        }

        stopwatch.Stop();
        ExecutionTime = stopwatch.Elapsed;
    }
}
