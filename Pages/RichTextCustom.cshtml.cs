using System.Diagnostics;
using DeliveryDemo.Models;
using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Delivery.ContentItems.RichText.Resolution;
using Kontent.Ai.Delivery.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryDemo.Pages;

public class RichTextCustomModel : PageModel
{
    private readonly IDeliveryClient _client;
    private readonly ILogger<RichTextCustomModel> _logger;

    public RichTextCustomModel(IDeliveryClient client, ILogger<RichTextCustomModel> logger)
    {
        _client = client;
        _logger = logger;
    }

    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public Article? Article { get; set; }
    public string? CustomResolvedHtml { get; set; }
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
                    // Build custom resolver
                    var resolver = new HtmlResolverBuilder()
                        .WithContentItemLinkResolver("article", async (link, resolveChildren) =>
                        {
                            var innerHtml = await resolveChildren(link.Children);
                            var url = $"/articles/{link.Metadata?.UrlSlug ?? link.Metadata?.Codename}";
                            return $"<a href=\"{url}\" class=\"article-link\">{innerHtml}</a>";
                        })
                        .WithContentResolver("tweet", content =>
                        {
                            var codename = content.Codename;
                            return $"<div class=\"tweet-embed\"><blockquote>Tweet: {codename}</blockquote></div>";
                        })
                        .WithContentResolver("hosted_video", content =>
                        {
                            var codename = content.Codename;
                            return $"<div class=\"video-wrapper\">Video: {codename}</div>";
                        })
                        .Build();

                    CustomResolvedHtml = await Article.BodyCopy.ToHtmlAsync(resolver);
                    _logger.LogInformation("Rendered rich text with custom resolvers for article '{Codename}'", Codename);
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
