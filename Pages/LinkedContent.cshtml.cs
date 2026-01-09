using Kontent.Ai.Delivery.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryDemo.Pages;

public class LinkedContentModel : PageModel
{
    private readonly IDeliveryClient _client;
    private readonly ILogger<LinkedContentModel> _logger;

    public LinkedContentModel(IDeliveryClient client, ILogger<LinkedContentModel> logger)
    {
        _client = client;
        _logger = logger;
    }

    public bool Depth0Success { get; set; }
    public bool Depth1Success { get; set; }
    public bool Depth2Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IContentItem<IDynamicElements>? Depth0Item { get; set; }
    public IContentItem<IDynamicElements>? Depth1Item { get; set; }
    public IContentItem<IDynamicElements>? Depth2Item { get; set; }
    public string Codename { get; set; } = "coffee_beverages_explained";

    public async Task OnGetAsync(string? codename = null)
    {
        Codename = codename ?? "coffee_beverages_explained";

        // Depth 0
        try
        {
            var result = await _client.GetItem(Codename)
                .Depth(0)
                .ExecuteAsync();

            if (result.IsSuccess)
            {
                Depth0Item = result.Value;
                Depth0Success = true;
                _logger.LogInformation("Retrieved item with depth 0: {Codename}", Codename);
            }
            else
            {
                ErrorMessage = result.Error?.Message;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error retrieving item with depth 0");
        }

        // Depth 1
        try
        {
            var result = await _client.GetItem(Codename)
                .Depth(1)
                .ExecuteAsync();

            if (result.IsSuccess)
            {
                Depth1Item = result.Value;
                Depth1Success = true;
                _logger.LogInformation("Retrieved item with depth 1: {Codename}",
                    Codename);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving item with depth 1");
        }

        // Depth 2
        try
        {
            var result = await _client.GetItem(Codename)
                .Depth(2)
                .ExecuteAsync();

            if (result.IsSuccess)
            {
                Depth2Item = result.Value;
                Depth2Success = true;
                _logger.LogInformation("Retrieved item with depth 2: {Codename}",
                    Codename);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving item with depth 2");
        }
    }
}
