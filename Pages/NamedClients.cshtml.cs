using Kontent.Ai.Delivery.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryDemo.Pages;

public class NamedClientsModel : PageModel
{
    private readonly IDeliveryClient _defaultClient;
    private readonly IDeliveryClientFactory _clientFactory;
    private readonly ILogger<NamedClientsModel> _logger;

    public NamedClientsModel(
        IDeliveryClient defaultClient,
        IDeliveryClientFactory clientFactory,
        ILogger<NamedClientsModel> logger)
    {
        _defaultClient = defaultClient;
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public bool ProductionSuccess { get; set; }
    public bool PreviewSuccess { get; set; }
    public bool DefaultSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public IContentItem<IDynamicElements>? ProductionItem { get; set; }
    public IContentItem<IDynamicElements>? PreviewItem { get; set; }
    public IContentItem<IDynamicElements>? DefaultItem { get; set; }

    public async Task OnGetAsync()
    {
        var codename = "coffee_beverages_explained";

        // Get production client
        try
        {
            var productionClient = _clientFactory.Get("production");
            var result = await productionClient.GetItem(codename).ExecuteAsync();

            if (result.IsSuccess)
            {
                ProductionItem = result.Value;
                ProductionSuccess = true;
                _logger.LogInformation("Retrieved item from production client");
            }
            else
            {
                ErrorMessage = result.Error?.Message;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error retrieving from production client");
        }

        // Get preview client
        try
        {
            var previewClient = _clientFactory.Get("preview");
            var result = await previewClient.GetItem(codename).ExecuteAsync();

            if (result.IsSuccess)
            {
                PreviewItem = result.Value;
                PreviewSuccess = true;
                _logger.LogInformation("Retrieved item from preview client");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving from preview client (may not be configured)");
        }

        // Get default client (not using factory)
        try
        {
            var result = await _defaultClient.GetItem(codename).ExecuteAsync();

            if (result.IsSuccess)
            {
                DefaultItem = result.Value;
                DefaultSuccess = true;
                _logger.LogInformation("Retrieved item from default client");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving from default client");
        }
    }
}
