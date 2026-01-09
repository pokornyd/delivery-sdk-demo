using System.Diagnostics;
using Kontent.Ai.Delivery.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryDemo.Pages;

public class MultipleItemsModel : PageModel
{
    private readonly IDeliveryClient _client;
    private readonly ILogger<MultipleItemsModel> _logger;

    public MultipleItemsModel(IDeliveryClient client, ILogger<MultipleItemsModel> logger)
    {
        _client = client;
        _logger = logger;
    }

    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public List<IContentItem<IDynamicElements>>? Items { get; set; }
    public int? TotalCount { get; set; }
    public int Skip { get; set; }
    public int Limit { get; set; } = 10;
    public TimeSpan ExecutionTime { get; set; }

    public async Task OnGetAsync(int skip = 0, int limit = 10)
    {
        Skip = skip;
        Limit = limit;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await _client.GetItems()
                .Where(f => f.System("type").IsEqualTo("article"))
                .OrderBy("system.last_modified", OrderingMode.Descending)
                .Skip(Skip)
                .Limit(Limit)
                .WithTotalCount()
                .ExecuteAsync();

            IsSuccess = result.IsSuccess;
            if (result.IsSuccess)
            {
                Items = result.Value.ToList();
                TotalCount = null; // TODO: TotalCount access needs investigation
                _logger.LogInformation("Successfully retrieved {Count} items (Total: {Total})", Items.Count, TotalCount);
            }
            else
            {
                ErrorMessage = result.Error?.Message ?? "Unknown error";
                _logger.LogWarning("Failed to retrieve items: {Error}", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _logger.LogError(ex, "Exception while retrieving items");
        }

        stopwatch.Stop();
        ExecutionTime = stopwatch.Elapsed;
    }
}
