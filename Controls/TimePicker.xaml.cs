using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic.Devices;

namespace ScheduledReplication.Controls;

public partial class TimePicker : UserControl
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public TimePicker()
    {
        InitializeComponent();
        // 填充小时和分钟的ComboBox
        FillHours();
        FillMinutes();
    }

    /// <summary>
    /// 小时依赖属性
    /// </summary>
    public static readonly DependencyProperty HourProperty = DependencyProperty.Register(
        nameof(Hour),
        typeof(int),
        typeof(TimePicker),
        new FrameworkPropertyMetadata(
            0,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            ValueChanged));

    /// <summary>
    /// 小时依赖属性包装器
    /// </summary>
    public int Hour
    {
        get => (int)GetValue(HourProperty);
        set => SetValue(HourProperty, value);
    }

    /// <summary>
    /// 分钟依赖属性
    /// </summary>
    public static readonly DependencyProperty MinuteProperty = DependencyProperty.Register(
        nameof(Minute),
        typeof(int),
        typeof(TimePicker),
        new FrameworkPropertyMetadata(
            0,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            ValueChanged));

    /// <summary>
    /// 分钟依赖属性包装器
    /// </summary>
    public int Minute
    {
        get => (int)GetValue(MinuteProperty);
        set => SetValue(MinuteProperty, value);
    }

    // 当依赖属性发生变化时，调用ValueChanged方法
    private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((TimePicker)d).UpdateRender();
    }

    // 渲染小时和分钟的ComboBox
    private void UpdateRender()
    {
        HourComboBox.SelectedIndex = Hour;
        MinutesComboBox.SelectedIndex = Minute;
    }

    private void FillHours()
    {
        for (var i = 0; i < 24; i++)
        {
            HourComboBox.Items.Add(i.ToString("00"));
        }
    }

    private void FillMinutes()
    {
        for (var i = 0; i < 60; i++)
        {
            MinutesComboBox.Items.Add(i.ToString("00"));
        }
    }

    private void TimeSelection(object? sender, EventArgs e)
    {
        Hour = int.Parse((HourComboBox.SelectedItem as string) ?? "0");
        Minute = int.Parse((MinutesComboBox.SelectedItem as string) ?? "0");
        HourComboBox.Focusable = false;
        MinutesComboBox.Focusable = false;
    }
}