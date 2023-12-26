using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ScheduledReplication.Tools;

public class ComplexSettings : IDisposable
{
    /// <summary>
    /// 配置文件名
    /// </summary>
    private const string FileName = "/ComplexSettings.json";

    /// <summary>
    /// 配置文件
    /// </summary>
    private readonly JObject settings;

    /// <summary>
    /// 构造
    /// </summary>
    public ComplexSettings()
    {
        try
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + FileName;
            // 文件是否存在
            if (!File.Exists(path))
            {
                settings = new JObject();
                Save();
                return;
            }

            var file = File.OpenText(path);
            settings = (JObject)JToken.ReadFrom(new JsonTextReader(file));
            file.Close();
        }
        catch
        {
            settings = new JObject();
            Save();
        }
    }

    /// <summary>
    /// 文件保存
    /// </summary>
    private void Save()
    {
        try
        {
            var fileWriter = File.CreateText(AppDomain.CurrentDomain.BaseDirectory + FileName);
            var writer = new JsonTextWriter(fileWriter);
            settings.WriteTo(writer);
            fileWriter.Close();
            writer.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            MessageBox.Show("配置文件保存失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>值</returns>
    public T? GetConfiguration<T>(string key)
    {
        try
        {
            if (settings.GetValue(key) == null)
            {
                return default;
            }

            var jToken = settings.GetValue(key);
            return jToken != null ? JsonConvert.DeserializeObject<T>(jToken.ToString()) : default;
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 设置配置
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public void SetConfiguration<T>(string key, T value)
    {
        try
        {
            var str = JsonConvert.SerializeObject(value, Formatting.Indented);
            if (!string.IsNullOrEmpty(str))
            {
                settings[key] = JToken.Parse(str);
            }

            Save();
        }
        catch
        {
            MessageBox.Show("配置文件保存失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// 释放
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}