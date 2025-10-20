using Realms;

namespace Misuzu.Database.Models;

public class LibraryEntry : RealmObject
{
    /// <summary>
    /// Unique ID of the entry in the database.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// The type of immersion this library entry provides.
    /// </summary>
    public int TypeValue { get; set; } = 0;

    // Not persisted — maps to TypeValue
    [Ignored]
    public ImmersionType Type
    {
        get => (ImmersionType)TypeValue;
        set => TypeValue = (int)value;
    }
    /// <summary>
    /// The title of the media in plain English text.
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// The description of the media stored in this entry.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// URL to a provider with metadata for the library entry
    /// </summary>
    public string ProviderUrl { get; set; } = string.Empty;
    /// <summary>
    /// The UUID used as the file name of the cover image representing this media.
    /// </summary>
    public string CoverImage { get; set; } = string.Empty;
    /// <summary>
    /// The UUID used as the file name of the banner image representing this media.
    /// </summary>
    public string BannerImage { get; set; } = string.Empty;
    /// <summary>
    /// The date the library entry was created at.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    /// <summary>
    /// Whether or not the library entry has been "completed", up to user discretion (i.e. watched all episodes of an anime, etc.)
    /// </summary>
    public bool Completed { get; set; } = false;
    /// <summary>
    /// The date at which the library entry was marked as completed. If unmarked, the value must be set to DateTimeOffset.MinValue
    /// </summary>
    public DateTimeOffset CompletedAt { get; set; } = DateTimeOffset.MinValue;
}
