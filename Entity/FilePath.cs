using ScheduledReplication.Tools;

namespace ScheduledReplication.Entity;

public class FilePath : ObservableObject
{
    private string? path;

    public string? Path
    {
        get => path;
        set => SetField(ref path, value);
    }
}