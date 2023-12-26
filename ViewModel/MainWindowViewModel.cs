using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ScheduledReplication.Entity;
using ScheduledReplication.Model;
using ScheduledReplication.Tools;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace ScheduledReplication.ViewModel;

/// <summary>
/// 主窗口视图模型
/// </summary>
public class MainWindowViewModel : IDisposable
{
    /// <summary>
    /// 配置
    /// </summary>
    private readonly Configuration configuration;

    /// <summary>
    /// 复杂设置
    /// </summary>
    private readonly ComplexSettings complexSettings = new();

    /// <summary>
    /// 定时任务 
    /// </summary>
    private ScheduledTask? scheduledTask;

    /// <summary>
    /// 文件列表
    /// </summary>
    public readonly ObservableCollection<FilePath> filePaths = new();

    /// <summary>
    /// 本地设置属性
    /// </summary>
    public LocalSettings LocalSettings { get; }

    /// <summary>
    /// 构造
    /// </summary>
    public MainWindowViewModel()
    {
        // 初始化命令
        SaveSettingsCommand = new ObservableCommand<object>(SaveSettings);
        AddResourcesCommand = new ObservableCommand<object>(AddResources);
        DeleteSelectedItemsCommand = new ObservableCommand<string>(DeleteSelectedItems);
        ChangeTargetAddressCommand = new ObservableCommand<object>(ChangeTargetAddress);

        // 读取配置
        configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        if (configuration.Sections["localSettings"] == null)
        {
            configuration.Sections.Add("localSettings", new LocalSettings());
            configuration.Save();
        }

        LocalSettings = (LocalSettings)configuration.GetSection("localSettings");

        // 读取复杂配置
        var list = complexSettings.GetConfiguration<string[]>("filePaths");
        if (list is { Length: > 0 })
            foreach (var path in list)
                filePaths.Add(new FilePath { Path = path });

        // 监听文件列表变化
        filePaths.CollectionChanged += UpdateFile;

        TaskInit();
    }

    /// <summary>
    /// 任务初始化
    /// </summary>
    private void TaskInit()
    {
        // 设置每天的特定时间 
        var executionTime = new TimeSpan(LocalSettings.TaskHour, LocalSettings.TaskMinute, 0);
        // 创建任务
        scheduledTask = new ScheduledTask(executionTime, CopyFile);
    }

    /// <summary>
    /// 析构
    /// </summary>
    ~MainWindowViewModel()
    {
        Dispose(false);
    }

    /// <summary>
    /// 保存设置命令
    /// </summary>
    public ObservableCommand<object> SaveSettingsCommand { get; }

    /// <summary>
    /// 保存设置
    /// </summary>
    private void SaveSettings(object? _)
    {
        try
        {
            configuration.Save();
            var newTime = new TimeSpan(LocalSettings.TaskHour, LocalSettings.TaskMinute, 0);
            if (scheduledTask != null)
            {
                scheduledTask.ChangeExecutionTime(newTime);
                MessageBox.Show($"保存成功-下次执行时间更新为:{scheduledTask!.NextExecutionTime}", "提示");
                return;
            }

            MessageBox.Show("保存失败", "错误");
        }
        catch
        {
            MessageBox.Show("保存失败", "错误");
        }
    }

    /// <summary>
    /// 文件列表变化时更新配置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateFile(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var list = new string?[filePaths.Count];
        for (var i = 0; i < filePaths.Count; i++) list[i] = filePaths[i].Path;

        // 更新配置
        complexSettings.SetConfiguration("filePaths", list);
    }

    /// <summary>
    /// 添加资源命令
    /// </summary>
    public ObservableCommand<object> AddResourcesCommand { get; }

    /// <summary>
    /// 添加资源
    /// </summary>
    private void AddResources(object? _)
    {
        try
        {
            var openFileDialog = new OpenFileDialog
            {
                // 设置筛选器只允许选择 .mdb 文件
                Filter = "Access Database Files (*.mdb)|*.mdb"
            };

            var result = openFileDialog.ShowDialog();

            if (result != true) return;
            // 是否已经存在
            if (filePaths.Any(filePath => filePath.Path == openFileDialog.FileName))
            {
                MessageBox.Show("已经存在", "提示");
                return;
            }

            // 添加文件路径
            filePaths.Add(new FilePath { Path = openFileDialog.FileName });
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "错误");
        }
    }

    /// <summary>
    /// 删除选中的条目命令
    /// </summary>
    public ObservableCommand<string> DeleteSelectedItemsCommand { get; }

    /// <summary>
    /// 删除选中的条目
    /// </summary>
    private void DeleteSelectedItems(string selectedItem)
    {
        try
        {
            if (string.IsNullOrEmpty(selectedItem))
            {
                MessageBox.Show("请选择要删除的条目", "提示");
                return;
            }

            // 删除选中的条目
            filePaths.Remove(filePaths.First(filePath => filePath.Path == selectedItem));
        }
        catch
        {
            MessageBox.Show("删除失败", "错误");
        }
    }

    /// <summary>
    /// 复制文件
    /// </summary>
    private void CopyFile()
    {
        try
        {
            foreach (var item in filePaths)
            {
                if (!File.Exists(item.Path)) continue;
                var source = Path.GetFileName(item.Path);
                var target = Path.Combine(LocalSettings.TargetPath, source);
                File.Copy(item.Path, target, true);
            }
        }
        catch
        {
            MessageBox.Show("复制失败", "错误");
        }
    }

    /// <summary>
    /// 更改目标地址命令
    /// </summary>
    public ObservableCommand<object> ChangeTargetAddressCommand { get; }

    /// <summary>
    /// 更改目标地址
    /// </summary>
    private void ChangeTargetAddress(object _)
    {
        try
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result != DialogResult.OK) return;
            LocalSettings.TargetPath = folderBrowserDialog.SelectedPath;
            configuration.Save();
        }
        catch
        {
            MessageBox.Show("更改失败", "错误");
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing">处置</param>
    private void Dispose(bool disposing)
    {
        if (!disposing) return;

        complexSettings.Dispose();
    }
}