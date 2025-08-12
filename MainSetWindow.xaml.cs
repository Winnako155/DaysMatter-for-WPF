using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DaysMatterWPF
{
    /// <summary>
    /// MainSetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainSetWindow : Window
    {
        public MainSetWindow()
        {
            InitializeComponent();
            if(IsShortcutInStartup("倒数日"))
            {
                autoStartButton.Content = "开启";
            }
            else
            {
                autoStartButton.Content = "关闭";
            }

        }
        public bool IsShortcutInStartup(string shortcutName)
        {
            // 获取启动文件夹路径
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            // 构造快捷方式的完整路径
            string shortcutPath = System.IO.Path.Combine(startupPath, $"{shortcutName}.lnk");

            // 检查文件是否存在
            return System.IO.File.Exists(shortcutPath);
        }
        public bool CreateStartupShortcut(string shortcutName)
        {
            try
            {
                string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = System.IO.Path.Combine(startupPath, $"{shortcutName}.lnk");
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                // 检查是否已存在
                if (IsShortcutInStartup(shortcutName))
                {
                    System.IO.File.Delete(shortcutPath);
                    return false;
                }
                else
                {
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

                    shortcut.Description = "程序开机启动快捷方式";
                    shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(exePath);
                    shortcut.TargetPath = exePath;
                    // 可选：设置图标
                    shortcut.IconLocation = exePath; // 使用程序自身图标
                    shortcut.Save();
                    return true;
                }
                
            }
            catch (Exception ex)
            {
                // 可以记录日志或提示用户
                System.Diagnostics.Debug.WriteLine($"创建快捷方式失败: {ex.Message}");
                return false;
            }
        }
        private void topBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void autoStartButton_Click(object sender, RoutedEventArgs e)
        {
            CreateStartupShortcut("倒数日");
            if (IsShortcutInStartup("倒数日"))
            {
                autoStartButton.Content = "开启";
            }
            else
            {
                autoStartButton.Content = "关闭";
            }
        }
    }
}
