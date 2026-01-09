using System.Diagnostics;
using DeliveryDemo.Models;
using Kontent.Ai.Delivery.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryDemo.Pages;

public class TypedModelsPageModel : PageModel
{
    private readonly IDeliveryClient _client;
    private readonly ILogger<TypedModelsPageModel> _logger;

    public TypedModelsPageModel(IDeliveryClient client, ILogger<TypedModelsPageModel> logger)
    {
        _client = client;
        _logger = logger;
    }

    public bool ArticlesSuccess { get; set; }
    public bool CoffeesSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public List<IContentItem<Article>>? Articles { get; set; }
    public List<IContentItem<Coffee>>? Coffees { get; set; }
    public TimeSpan ArticlesTime { get; set; }
    public TimeSpan CoffeesTime { get; set; }

    public async Task OnGetAsync()
    {
        // Get articles
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var articlesResult = await _client.GetItems<Article>()
                .Where(f => f.System("type").IsEqualTo("article"))
                .Limit(3)
                .ExecuteAsync();

            stopwatch.Stop();
            ArticlesTime = stopwatch.Elapsed;

            if (articlesResult.IsSuccess)
            {
                Articles = articlesResult.Value.ToList();
                ArticlesSuccess = true;
                _logger.LogInformation("Retrieved {Count} articles", Articles.Count);
            }
            else
            {
                ErrorMessage = articlesResult.Error?.Message ?? "Unknown error";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _logger.LogError(ex, "Exception while retrieving articles");
        }

        // Get coffees
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var coffeesResult = await _client.GetItems<Coffee>()
                .Where(f => f.System("type").IsEqualTo("coffee"))
                .Limit(3)
                .ExecuteAsync();

            stopwatch.Stop();
            CoffeesTime = stopwatch.Elapsed;

            if (coffeesResult.IsSuccess)
            {
                Coffees = coffeesResult.Value.ToList();
                CoffeesSuccess = true;
                _logger.LogInformation("Retrieved {Count} coffees", Coffees.Count);
            }
            else if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = coffeesResult.Error?.Message ?? "Unknown error";
            }
        }
        catch (Exception ex)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = ex.Message;
            }
            _logger.LogError(ex, "Exception while retrieving coffees");
        }
    }
}
