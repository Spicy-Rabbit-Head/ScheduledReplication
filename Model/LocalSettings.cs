using System;
using System.Configuration;
using ScheduledReplication.Tools;

namespace ScheduledReplication.Model;

/// <summary>
/// 本地设置
/// </summary>
public class LocalSettings : ObservableObject
{
    /// <summary>
    /// 任务小时本地设置
    /// </summary>
    [ConfigurationProperty("taskHour", DefaultValue = 0)]
    public int TaskHour
    {
        get => (int)this["taskHour"];
        set => this["taskHour"] = value;
    }


    /// <summary>
    /// 任务分钟本地设置
    /// </summary>
    [ConfigurationProperty("taskMinute", DefaultValue = 0)]
    public int TaskMinute
    {
        get => (int)this["taskMinute"];
        set => this["taskMinute"] = value;
    }

    private string targetPath = string.Empty;

    /// <summary>
    /// 目标地址本地设置
    /// </summary>
    [ConfigurationProperty("targetPath", DefaultValue = "")]
    public string TargetPath
    {
        get
        {
            if (string.IsNullOrEmpty((string)this["targetPath"]))
                return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            return (string)this["targetPath"];
        }
        set
        {
            this["targetPath"] = value;
            SetField(ref targetPath, value);
        }
    }
}