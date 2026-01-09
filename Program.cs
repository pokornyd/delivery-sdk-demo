using Kontent.Ai.Delivery.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure default Delivery Client with memory caching
builder.Services.AddDeliveryClient(
    builder.Configuration,
    "DeliveryOptions");
    //.WithMemoryCache(TimeSpan.FromMinutes(5));

// Configure named clients for production and preview
builder.Services.AddDeliveryClient("production", options =>
{
    options.EnvironmentId = builder.Configuration["DeliveryOptions:EnvironmentId"]
        ?? "975bf280-fd91-488c-994c-2f04416e5ee3";
    options.UsePreviewApi = false;
});

builder.Services.AddDeliveryClient("preview", options =>
{
    options.EnvironmentId = builder.Configuration["DeliveryOptions:EnvironmentId"]
        ?? "975bf280-fd91-488c-994c-2f04416e5ee3";
    options.UsePreviewApi = true;
    options.PreviewApiKey = builder.Configuration["DeliveryOptions:PreviewApiKey"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
