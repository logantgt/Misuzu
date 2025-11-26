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

Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "store/"));

builder.Services.AddScoped(_ =>
{
    var config = new RealmConfiguration(Path.Combine(Environment.CurrentDirectory, "store/"))
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