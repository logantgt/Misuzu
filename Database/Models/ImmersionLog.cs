using Realms;

namespace Misuzu.Database.Models;

public class ImmersionLog : RealmObject
{
    /// <summary>
    /// The unique ID of this immersion log entry.
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
    /// The amount of media consumed in this immersion log entry.
    /// </summary>
    public int Amount { get; set; } = 0;
    /// <summary>
    /// The duration of media consumption in this immersion log entry.
    /// </summary>
    public long DurationTicks { get; set; }

    [Ignored]
    public TimeSpan Duration
    {
        get => TimeSpan.FromTicks(DurationTicks);
        set => DurationTicks = value.Ticks;
    }
    /// <summary>
    /// The creation timestamp of this immersion log entry.
    /// </summary>
    public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;
    /// <summary>
    /// An identifier for the content being logged in this immersion log entry. (Library Entry)
    /// </summary>
    public string Content { get; set; } = string.Empty;
    /// <summary>
    /// A comment note added by the user to describe the immersion log.
    /// </summary>
    public string Comment { get; set; } = string.Empty;
}