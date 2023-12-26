using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace ScheduledReplication.Tools;

/// <summary>
/// 可观察对象
/// </summary>
public class ObservableObject : ConfigurationSection, INotifyPropertyChanged
{
    /// <summary>
    /// 属性改变事件
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 属性改变事件
    /// </summary>
    /// <param name="propertyName">属性名称</param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// 设置字段
    /// </summary>
    /// <param name="field">字段</param>
    /// <param name="value">值</param>
    /// <param name="propertyName">属性名称</param>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>是否设置成功</returns>
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}