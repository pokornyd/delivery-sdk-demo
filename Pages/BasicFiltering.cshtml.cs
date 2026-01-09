using System.Diagnostics;
using Kontent.Ai.Delivery.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryDemo.Pages;

public class BasicFilteringModel : PageModel
{
    private readonly IDeliveryClient _client;
    private readonly ILogger<BasicFilteringModel> _logger;

    public BasicFilteringModel(IDeliveryClient client, ILogger<BasicFilteringModel> logger)
    {
        _client = client;
        _logger = logger;
    }

    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public List<IContentItem<IDynamicElements>>? Items { get; set; }
    public TimeSpan ExecutionTime { get; set; }

    public async Task OnGetAsync()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await _client.GetItems()
                .Where(f => f
                    .System("type").IsEqualTo("article")
                    .Element("title").Contains("coffee"))
                .Limit(10)
                .ExecuteAsync();

            IsSuccess = result.IsSuccess;
            if (result.IsSuccess)
            {
                Items = result.Value.ToList();
                _logger.LogInformation("Successfully retrieved {Count} filtered items", Items.Count);
            }
            else
            {
                ErrorMessage = result.Error?.Message ?? "Unknown error";
                _logger.LogWarning("Failed to retrieve filtered items: {Error}", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _logger.LogError(ex, "Exception while retrieving filtered items");
        }

        stopwatch.Stop();
        ExecutionTime = stopwatch.Elapsed;
    }
}
