using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Atc.Cosmos.EventStore.Diagnostics;

public static class ActivityExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RecordException(this Activity activity, Exception? ex)
    {
        TagList tags = default(TagList);
        activity.RecordException(ex, in tags);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RecordException(this Activity activity, Exception? ex, in TagList tags)
    {
        if (ex == null || activity == null)
        {
            return;
        }

        ActivityTagsCollection activityTagsCollection = new ActivityTagsCollection
        {
            { "exception.type", ex.GetType().FullName },
            { "exception.stacktrace", ex.ToString() },
        };

        if (!string.IsNullOrWhiteSpace(ex.Message))
        {
            activityTagsCollection.Add("exception.message", ex.Message);
        }

        foreach (KeyValuePair<string, object?> tag in tags)
        {
            activityTagsCollection[tag.Key] = tag.Value;
        }

        activity.AddEvent(new ActivityEvent("exception", default(DateTimeOffset), activityTagsCollection));
    }
}