using System;
using System.Threading;

namespace ScheduledReplication.Tools;

/// <summary>
/// 定时任务
/// </summary>
public class ScheduledTask : IDisposable
{
    /// <summary>
    /// 执行时间
    /// </summary>
    private TimeSpan executionTime;

    /// <summary>
    /// 任务
    /// </summary>
    private readonly Action taskAction;

    /// <summary>
    /// 定时器
    /// </summary>
    private Timer? timer;

    /// <summary>
    /// 下次执行时间
    /// </summary>
    private DateTime nextExecutionTime;

    /// <summary>
    /// 下次执行时间属性
    /// </summary>
    public DateTime NextExecutionTime
    {
        get => nextExecutionTime;
        private set => nextExecutionTime = value;
    }

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="executionTime">执行时间</param>
    /// <param name="taskAction">任务</param>
    public ScheduledTask(TimeSpan executionTime, Action taskAction)
    {
        this.executionTime = executionTime;
        this.taskAction = taskAction;

        SetTimer();
    }

    /// <summary>
    /// 设置定时器
    /// </summary>
    private void SetTimer()
    {
        var now = DateTime.Now;
        var scheduledTime = DateTime.Today.Add(executionTime);

        if (scheduledTime < now) scheduledTime = scheduledTime.AddDays(1);
        var timeToGo = scheduledTime - now;
        NextExecutionTime = scheduledTime;

        timer = new Timer(_ =>
        {
            taskAction.Invoke();
            // 改时间为第二天
            SetTimer();
        }, null, timeToGo, TimeSpan.FromDays(1));
    }

    /// <summary>
    /// 更改执行时间并重置定时器
    /// </summary>
    /// <param name="newExecutionTime">新的执行时间</param>
    public void ChangeExecutionTime(TimeSpan newExecutionTime)
    {
        // 停止当前定时器
        timer?.Change(Timeout.Infinite, Timeout.Infinite);

        // 更新执行时间
        executionTime = newExecutionTime;

        // 重新设置定时器
        SetTimer();
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        timer?.Dispose();
        GC.SuppressFinalize(this);
    }
}