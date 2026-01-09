using System.Diagnostics;
using DeliveryDemo.Models;
using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Delivery.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryDemo.Pages;

public class RichTextBasicModel : PageModel
{
    private readonly IDeliveryClient _client;
    private readonly ILogger<RichTextBasicModel> _logger;

    public RichTextBasicModel(IDeliveryClient client, ILogger<RichTextBasicModel> logger)
    {
        _client = client;
        _logger = logger;
    }

    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public Article? Article { get; set; }
    public string? HtmlContent { get; set; }
    public string Codename { get; set; } = "coffee_beverages_explained";
    public TimeSpan ExecutionTime { get; set; }

    public async Task OnGetAsync(string? codename = null)
    {
        Codename = codename ?? "coffee_beverages_explained";
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await _client.GetItem<Article>(Codename)
                .Depth(1)
                .ExecuteAsync();

            IsSuccess = result.IsSuccess;
            if (result.IsSuccess)
            {
                Article = result.Value.Elements;

                if (Article.BodyCopy != null)
                {
                    HtmlContent = await Article.BodyCopy.ToHtmlAsync();
                    _logger.LogInformation("Rendered rich text for article '{Codename}'", Codename);
                }
                else
                {
                    _logger.LogInformation("Article '{Codename}' has no body copy", Codename);
                }
            }
            else
            {
                ErrorMessage = result.Error?.Message ?? "Unknown error";
                _logger.LogWarning("Failed to retrieve article: {Error}", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _logger.LogError(ex, "Exception while retrieving article");
        }

        stopwatch.Stop();
        ExecutionTime = stopwatch.Elapsed;
    }
}
