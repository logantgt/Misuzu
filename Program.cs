using Misuzu.Components;
using Misuzu.Services;
using Realms;
using ApexCharts;
using Misuzu.Database.Models;
using Misuzu.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

var storePath = Path.Combine(Environment.CurrentDirectory, "store/");
Directory.CreateDirectory(storePath);

// Clean up stale Realm lock files (fixes "No such process" error on Windows)
var lockFiles = Directory.GetFiles(storePath, "*.lock");
var managementFiles = Directory.GetFiles(storePath, "*.management");
foreach (var file in lockFiles.Concat(managementFiles))
{
    try
    {
        File.Delete(file);
    }
    catch
    {
        // Ignore errors - if we can't delete, Realm will handle it
    }
}

builder.Services.AddScoped(_ =>
{
    var config = new RealmConfiguration(storePath)
    {
        SchemaVersion = 3
    };
    return Realm.GetInstance(config);
});

builder.Services.AddScoped<RealmDbContext>();

// THIS SHOULD REALLY BE A SINGLETON!!!!!!!!!!!!!!!!!!
builder.Services.AddScoped<WebhookService>();

builder.Services.AddSingleton<SettingsService>();

builder.Services.AddApexCharts();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();