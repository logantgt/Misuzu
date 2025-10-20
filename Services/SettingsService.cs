using System.Text.Json;

namespace Misuzu.Services;
public class SettingsService
{
    private const string FileName = "settings.json";
    private readonly string _filePath;

    public AppSettings Settings { get; private set; } = new();

    public SettingsService(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, FileName);
        Load();
    }
    public void Load()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            Settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        else
        {
            Settings = new AppSettings();
            Save();
        }
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    public void AddWebhook(WebhookEntry entry)
    {
        Settings.WebhookEntries.Add(entry);
        Save();
    }

    public void RemoveWebhook(WebhookEntry entry)
    {
        if (Settings.WebhookEntries.Remove(entry))
        {
            Save();
        }
    }

    public void UpdateWebhook(WebhookEntry entry)
    {
        Save(); // assuming changes are already applied to the entry object
    }
}
