using Kontent.Ai.Delivery.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryDemo.Pages;

public class MultiLanguageModel : PageModel
{
    private readonly IDeliveryClient _client;
    private readonly ILogger<MultiLanguageModel> _logger;

    public MultiLanguageModel(IDeliveryClient client, ILogger<MultiLanguageModel> logger)
    {
        _client = client;
        _logger = logger;
    }

    public bool LanguagesSuccess { get; set; }
    public bool EnglishSuccess { get; set; }
    public bool SpanishSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public IReadOnlyList<ILanguage>? Languages { get; set; }
    public IContentItem<IDynamicElements>? EnglishItem { get; set; }
    public IContentItem<IDynamicElements>? SpanishItem { get; set; }

    public async Task OnGetAsync()
    {
        // Get available languages
        try
        {
            var languagesResult = await _client.GetLanguages().ExecuteAsync();

            if (languagesResult.IsSuccess)
            {
                Languages = languagesResult.Value;
                LanguagesSuccess = true;
                _logger.LogInformation("Retrieved {Count} languages", Languages.Count);
            }
            else
            {
                ErrorMessage = languagesResult.Error?.Message;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error retrieving languages");
        }

        // Get English variant
        try
        {
            var enResult = await _client.GetItem("on_roasts")
                .WithLanguage("en-US")
                .ExecuteAsync();

            if (enResult.IsSuccess)
            {
                EnglishItem = enResult.Value;
                EnglishSuccess = true;
                _logger.LogInformation("Retrieved English variant");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving English variant");
        }

        // Get Spanish variant
        try
        {
            var esResult = await _client.GetItem("on_roasts")
                .WithLanguage("es-ES")
                .ExecuteAsync();

            if (esResult.IsSuccess)
            {
                SpanishItem = esResult.Value;
                SpanishSuccess = true;
                _logger.LogInformation("Retrieved Spanish variant");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Spanish variant");
        }
    }
}
