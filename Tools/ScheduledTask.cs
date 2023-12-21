using System;
using System.Threading;

namespace ScheduledReplication.ScheduledTasks;

public class ScheduledTask
{
    private TimeSpan executionTime;
    private readonly Action taskAction;
    private Timer? timer;

    public ScheduledTask(TimeSpan executionTime, Action taskAction)
    {
        this.executionTime = executionTime;
        this.taskAction = taskAction;

        SetTimer();
    }

    private void SetTimer()
    {
        DateTime now = DateTime.Now;
        DateTime scheduledTime = DateTime.Today.Add(executionTime);
        Console.WriteLine(scheduledTime);
        Console.WriteLine(executionTime);
        if (scheduledTime < now)
        {
            scheduledTime = scheduledTime.AddSeconds(20);
            executionTime = executionTime.Add(new TimeSpan(0, 0, 20));
        }

        TimeSpan timeToGo = scheduledTime - now;
        Console.WriteLine(scheduledTime);
        Console.WriteLine(executionTime);

        timer = new Timer(x =>
        {
            taskAction.Invoke();
            SetTimer();
            // 改时间为第二天
        }, null, timeToGo, TimeSpan.FromDays(1));
    }
}