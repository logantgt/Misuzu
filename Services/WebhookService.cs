using Misuzu.Database.Models;
using Scriban;
using System.Net.Mime;
using System.Net.Http;
using Realms;
using Misuzu.Database;
using System.Text;
using System;

namespace Misuzu.Services;

public class WebhookService
{
    private SettingsService _settings;
    private HttpClient _client;
    private RealmDbContext _context;
    public WebhookService(SettingsService settings, RealmDbContext context)
    {
        _settings = settings;
        _client = new HttpClient();
        _context = context;
    }
    public void Dispatch(ImmersionLog log, WebhookAction action)
    {
        foreach (WebhookEntry webhook in _settings.Settings.WebhookEntries)
        {
            if (webhook.Trigger == action)
            {
                FireWebhook(webhook, log);
            }
        }
    }

    private async Task FireWebhook(WebhookEntry webhook, ImmersionLog log)
    {
        Template contentBodyTemplate = Template.Parse(webhook.JsonBody);
        LibraryEntry entrySnapshot;

        var entry = _context.GetLibraryEntries().FirstOrDefault(e => e.Id == log.Content);

        entrySnapshot = new LibraryEntry
        {
            Id = entry.Id,
            Title = entry.Title,
            Description = entry.Description,
            Type = entry.Type,
            CoverImage = entry.CoverImage,
            BannerImage = entry.BannerImage,
            ProviderUrl = entry.ProviderUrl,
            Completed = entry.Completed
        };

        string verb = "";
        string unit = "";

        // Sorry, only english for now
        switch (entrySnapshot.Type)
        {
            case ImmersionType.Anime:
            case ImmersionType.Movie:
            case ImmersionType.TV:
            case ImmersionType.Video:
                verb = "Watched";
                break;
            case ImmersionType.Audiobook:
            case ImmersionType.Listening:
                verb = "Listened to";
                break;
            case ImmersionType.Book:
            case ImmersionType.Comic:
            case ImmersionType.Manga:
            case ImmersionType.VisualNovel:
                verb = "Read";
                break;
            case ImmersionType.Other:
            case ImmersionType.Study:
                verb = "Studied";
                break;
        }

        // This is very opinionated and should be left as a user facing option
        // when adding a library entry
        switch (entrySnapshot.Type)
        {
            case ImmersionType.Anime:
            case ImmersionType.Movie:
            case ImmersionType.TV:
            case ImmersionType.Video:
                unit = "episodes";
                break;
            case ImmersionType.Audiobook:
            case ImmersionType.Listening:
                unit = "chapters";
                break;
            case ImmersionType.Book:
            case ImmersionType.Comic:
            case ImmersionType.Manga:
            case ImmersionType.VisualNovel:
                unit = "characters";
                break;
            case ImmersionType.Other:
            case ImmersionType.Study:
                unit = "cards";
                break;
        }

        int streak = 1;

        DateTime lastDate = DateTime.Now.Date;

        var logs = _context.GetImmersionLogs().ToList();
        logs.Reverse();

        foreach (ImmersionLog logged in logs)
        {
            if (logged.TimeStamp.Date >= lastDate) continue;

            if (lastDate.Subtract(TimeSpan.FromDays(1)) == logged.TimeStamp.Date)
            {
                streak++;
            }
            else
            {
                break;
            }

            lastDate = logged.TimeStamp.Date;
        }

        // TODO: don't hardcode this path
        var file = Directory.EnumerateFiles("store/images")
                .FirstOrDefault(f =>
                    string.Equals(Path.GetFileNameWithoutExtension(f), entrySnapshot.CoverImage, StringComparison.OrdinalIgnoreCase));

        string result = contentBodyTemplate.Render(new
        {
            content_id = entrySnapshot.Id,
            content_type = entrySnapshot.Type.ToString(),
            content_title = entrySnapshot.Title,
            content_description = entrySnapshot.Description,
            content_provider_url = entrySnapshot.ProviderUrl,
            content_cover_image = entrySnapshot.CoverImage,
            content_banner_image = entrySnapshot.BannerImage,
            content_created_at = entrySnapshot.CreatedAt,
            content_completed = entrySnapshot.Completed,
            log_id = log.Id,
            log_amount = log.Amount,
            log_duration = log.Duration,
            log_timestamp = log.TimeStamp,
            log_content = log.Content,
            log_comment = log.Comment,
            meta_verb = verb,
            meta_unit = unit,
            meta_date = DateTime.Now.ToShortDateString(),
            meta_month = DateTime.Now.ToString("MMMM"),
            meta_streak = streak,
            meta_duration = $"{log.Duration.Hours}h {log.Duration.Minutes}m",
            meta_thumbnail = $"thumbnail{Path.GetExtension(file)}"
        });

        if (webhook.PayloadType == WebhookPayloadType.JSON)
        {
            await _client.PostAsync(webhook.Url, new StringContent(result, Encoding.UTF8, "application/json"));
        }
        else if (webhook.PayloadType == WebhookPayloadType.MultiPart)
        {
            MultipartFormDataContent form = new();

            form.Add(new StringContent(result), "payload_json");

            var image = File.ReadAllBytes(file);

            form.Add(new ByteArrayContent(image, 0, image.Length), "thumbnail", "thumbnail" + Path.GetExtension(file));

            await _client.PostAsync(webhook.Url, form);
        }
    }
}