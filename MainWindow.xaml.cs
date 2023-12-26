using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Hardcodet.Wpf.TaskbarNotification;
using ScheduledReplication.Tools;
using ScheduledReplication.ViewModel;
using Binding = System.Windows.Data.Binding;
using TextBox = System.Windows.Controls.TextBox;

namespace ScheduledReplication;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// 是否关闭
    /// </summary>
    private bool isClose;

    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    private readonly MainWindowViewModel mainViewModel = new();


    /// <summary>
    /// 托盘
    /// </summary>
    private readonly TaskbarIcon tray = new();

    /// <summary>
    /// 右键菜单
    /// </summary>
    private readonly ContextMenu contextMenu = new();


    /// <summary>
    /// 构造
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContext = mainViewModel;
        DocumentDisplay.ItemsSource = mainViewModel.filePaths;
        var binding = new Binding("SelectedItem.Path") { Source = DocumentDisplay };
        FileSelection.SetBinding(TextBox.TextProperty, binding);
        TrayInit();
    }

    /// <summary>
    /// 托盘初始化
    /// </summary>
    private void TrayInit()
    {
        // 设置托盘图标
        tray.IconSource = new BitmapImage(new Uri("pack://application:,,,/Icon/system.ico"));
        // 设置托盘工具提示文本
        tray.ToolTipText = "文件自动复制程序";
        // 托盘双击
        tray.TrayMouseDoubleClick += ShowInterface;

        // 添加显示界面菜单项
        var showMenuItem = new MenuItem
        {
            Header = "显示界面",
            Command = new ObservableCommand<object>(ShowInterface)
        };

        // 添加隐藏界面菜单项
        var hideMenuItem = new MenuItem
        {
            Header = "隐藏界面",
            Command = new ObservableCommand<object>(HideInterface)
        };

        // 添加退出程序菜单项
        var exitMenuItem = new MenuItem
        {
            Header = "退出程序",
            Command = new ObservableCommand<EventArgs>(CloseWindow)
        };
        contextMenu.Items.Add(showMenuItem);
        contextMenu.Items.Add(hideMenuItem);
        // 添加分隔线
        contextMenu.Items.Add(new Separator());
        contextMenu.Items.Add(exitMenuItem);
        // 右键菜单关联到 NotifyIcon
        tray.ContextMenu = contextMenu;
    }

    /// <summary>
    /// 显示界面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ShowInterface(object sender, RoutedEventArgs e)
    {
        ShowInterface(sender);
    }

    /// <summary>
    /// 显示界面
    /// </summary>
    private void ShowInterface(object _)
    {
        // 如果窗口最小化,则显示窗口
        if (WindowState == WindowState.Minimized)
        {
            Display();
            return;
        }

        // 如果窗口隐藏,则显示窗口
        if (!IsVisible) Display();
    }

    /// <summary>
    /// 显示
    /// </summary>
    private void Display()
    {
        WindowState = WindowState.Normal;
        Activate();
        Visibility = Visibility.Visible;
    }

    /// <summary>
    /// 隐藏界面
    /// </summary>
    private void HideInterface(object _)
    {
        HideInterface();
    }

    /// <summary>
    /// 隐藏界面
    /// </summary>
    private void HideInterface()
    {
        Visibility = Visibility.Hidden;
    }

    /// <summary>
    /// 窗口关闭
    /// </summary>
    private void CloseWindow(object _)
    {
        isClose = true;
        Close();
    }

    /// <summary>
    /// 窗口关闭前
    /// </summary>
    /// <param name="e"></param>
    protected override void OnClosing(CancelEventArgs e)
    {
        HideInterface();
        if (isClose)
        {
            base.OnClosing(e);
            return;
        }

        e.Cancel = true;
    }


    /// <summary>
    /// 窗口关闭后
    /// </summary>
    /// <param name="e"></param>
    protected override void OnClosed(EventArgs e)
    {
        // 释放视图模型
        mainViewModel.Dispose();
        // 隐藏托盘
        tray.Visibility = Visibility.Collapsed;
        // 释放托盘
        tray.Dispose();
        base.OnClosed(e);
    }
}