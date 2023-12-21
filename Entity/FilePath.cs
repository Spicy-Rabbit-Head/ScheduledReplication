using ScheduledReplication.Tools;

namespace ScheduledReplication;

public class FilePath : ObservableObject
{
    private string? path;

    public string? Path
    {
        get => path;
        set => SetField(ref path, value);
    }
}