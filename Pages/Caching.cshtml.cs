using System.Diagnostics;
using Kontent.Ai.Delivery.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace DeliveryDemo.Pages;

public class CachingModel : PageModel
{
    private readonly IDeliveryClient _client;
    private readonly ILogger<CachingModel> _logger;
    private readonly IMemoryCache? _memoryCache;

    public CachingModel(IDeliveryClient client, ILogger<CachingModel> logger, IMemoryCache? memoryCache = null)
    {
        _client = client;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public bool CachingEnabled { get; set; }
    public bool FirstCallSuccess { get; set; }
    public bool SecondCallSuccess { get; set; }
    public bool ThirdCallSuccess { get; set; }
    public bool FirstCallFromCache { get; set; }
    public bool SecondCallFromCache { get; set; }
    public bool ThirdCallFromCache { get; set; }
    public string? ErrorMessage { get; set; }
    public IContentItem<IDynamicElements>? FirstCallItem { get; set; }
    public IContentItem<IDynamicElements>? SecondCallItem { get; set; }
    public TimeSpan FirstCallTime { get; set; }
    public TimeSpan SecondCallTime { get; set; }
    public TimeSpan ThirdCallTime { get; set; }
    public double SpeedImprovement { get; set; }

    public async Task OnGetAsync()
    {
        var codename = "coffee_beverages_explained";

        // Check if caching is enabled by checking if IMemoryCache is registered
        CachingEnabled = _memoryCache != null;

        // Build the cache key (matches CacheKeyBuilder format: "item:{codename}:")
        var cacheKey = $"item:{codename}:";

        // First call - check cache state before call
        try
        {
            FirstCallFromCache = CheckCacheState(cacheKey);

            var stopwatch = Stopwatch.StartNew();
            var result = await _client.GetItem(codename).ExecuteAsync();
            stopwatch.Stop();

            FirstCallTime = stopwatch.Elapsed;

            if (result.IsSuccess)
            {
                FirstCallItem = result.Value;
                FirstCallSuccess = true;
                _logger.LogInformation("First call completed in {Ms}ms (from cache: {FromCache})",
                    FirstCallTime.TotalMilliseconds, FirstCallFromCache);
            }
            else
            {
                ErrorMessage = result.Error?.Message;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error on first call");
        }

        // Second call - check cache state before call
        try
        {
            SecondCallFromCache = CheckCacheState(cacheKey);

            var stopwatch = Stopwatch.StartNew();
            var result = await _client.GetItem(codename).ExecuteAsync();
            stopwatch.Stop();

            SecondCallTime = stopwatch.Elapsed;

            if (result.IsSuccess)
            {
                SecondCallItem = result.Value;
                SecondCallSuccess = true;

                // Calculate speed improvement
                if (SecondCallTime.TotalMilliseconds > 0)
                {
                    SpeedImprovement = FirstCallTime.TotalMilliseconds / SecondCallTime.TotalMilliseconds;
                }

                _logger.LogInformation("Second call completed in {Ms}ms (from cache: {FromCache})",
                    SecondCallTime.TotalMilliseconds, SecondCallFromCache);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on second call");
        }

        // Third call - check cache state before call
        try
        {
            ThirdCallFromCache = CheckCacheState(cacheKey);

            var stopwatch = Stopwatch.StartNew();
            var result = await _client.GetItem(codename).ExecuteAsync();
            stopwatch.Stop();

            ThirdCallTime = stopwatch.Elapsed;
            ThirdCallSuccess = result.IsSuccess;

            _logger.LogInformation("Third call completed in {Ms}ms (from cache: {FromCache})",
                ThirdCallTime.TotalMilliseconds, ThirdCallFromCache);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on third call");
        }
    }

    /// <summary>
    /// Checks if a value exists in the cache for the given key.
    /// </summary>
    /// <param name="cacheKey">The cache key to check.</param>
    /// <returns>True if the key exists in cache, false otherwise.</returns>
    private bool CheckCacheState(string cacheKey)
    {
        if (_memoryCache == null)
            return false;

        return _memoryCache.TryGetValue(cacheKey, out _);
    }
}
