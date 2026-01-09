using System.Diagnostics;
using Kontent.Ai.Delivery.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryDemo.Pages;

public class AdvancedFilteringModel : PageModel
{
    private readonly IDeliveryClient _client;
    private readonly ILogger<AdvancedFilteringModel> _logger;

    public AdvancedFilteringModel(IDeliveryClient client, ILogger<AdvancedFilteringModel> logger)
    {
        _client = client;
        _logger = logger;
    }

    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public List<IContentItem<IDynamicElements>>? Items { get; set; }
    public int? TotalCount { get; set; }
    public TimeSpan ExecutionTime { get; set; }

    public async Task OnGetAsync()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await _client.GetItems()
                .Where(f => f.System("type").IsEqualTo("article"))
                .OrderBy("system.last_modified", OrderingMode.Descending)
                .Limit(5)
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
