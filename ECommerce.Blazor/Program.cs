using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ECommerce.Blazor.Data;
using ECommerce.Blazor.Services;
using ECommerce.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Ensure MongoDB class mappings are configured at startup
MongoDbContext.EnsureClassMappingsConfigured();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configure MongoDB settings
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Register MongoDB context first to ensure class mapping is configured
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();

// Register services
builder.Services.AddScoped<ILogService, LogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
