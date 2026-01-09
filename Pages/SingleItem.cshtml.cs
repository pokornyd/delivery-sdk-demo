using System.Diagnostics;
using Kontent.Ai.Delivery.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryDemo.Pages;

public class SingleItemModel : PageModel
{
    private readonly IDeliveryClient _client;
    private readonly ILogger<SingleItemModel> _logger;

    public SingleItemModel(IDeliveryClient client, ILogger<SingleItemModel> logger)
    {
        _client = client;
        _logger = logger;
    }

    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public IContentItem<IDynamicElements>? Item { get; set; }
    public string Codename { get; set; } = "coffee_beverages_explained";
    public TimeSpan ExecutionTime { get; set; }

    public async Task OnGetAsync(string? codename = null)
    {
        Codename = codename ?? "coffee_beverages_explained";
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await _client.GetItem(Codename).ExecuteAsync();

            IsSuccess = result.IsSuccess;
            if (result.IsSuccess)
            {
                Item = result.Value;
                _logger.LogInformation("Successfully retrieved item '{Codename}'", Codename);
            }
            else
            {
                ErrorMessage = result.Error?.Message ?? "Unknown error";
                _logger.LogWarning("Failed to retrieve item '{Codename}': {Error}", Codename, ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _logger.LogError(ex, "Exception while retrieving item '{Codename}'", Codename);
        }

        stopwatch.Stop();
        ExecutionTime = stopwatch.Elapsed;
    }
}
