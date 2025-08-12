using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace DaysMatterWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window //主页逻辑
    {
        private System.Windows.Forms.NotifyIcon notifyIcon;
        public string toolsInfoLocation; //小组件地址
        public int seindex=-1; //选中的小组件的索引
        public string listInfoLocation = System.Environment.CurrentDirectory + @"\itemsList.inf"; //获取列表信息地址
        public ObservableCollection<string> items; //小组件列表(后端)
        public MainWindow()
        {
            InitializeComponent(); //初始化
            this.Hide();
            //创建托盘
            try
            {
                // 创建 NotifyIcon 实例
                notifyIcon = new System.Windows.Forms.NotifyIcon();
                // 使用 Pack URI 语法访问资源
                var iconUri = new Uri("pack://application:,,,/img/icon.ico");
                var iconStream = System.Windows.Application.GetResourceStream(iconUri);

                if (iconStream != null)
                {
                    using (var stream = iconStream.Stream)
                    {
                        notifyIcon.Icon = new System.Drawing.Icon(stream);
                    }
                }
                else
                {
                    // 如果资源未找到，使用默认图标
                    notifyIcon.Icon = System.Drawing.SystemIcons.Application;
                }
            }
            catch (Exception ex)
            {
                // 处理异常，例如使用默认图标
                notifyIcon.Icon = System.Drawing.SystemIcons.Application;
                System.Diagnostics.Debug.WriteLine($"加载图标失败: {ex.Message}");
            }


            // 设置提示文本
            notifyIcon.Text = "倒数日";

            // 显示图标
            notifyIcon.Visible = true;

            // 订阅双击事件
            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
            //创建托盘
            items = new ObservableCollection<string>();
            itemsBox.ItemsSource = items; //绑定列表关系
            if (File.Exists(listInfoLocation)) //如果文件存在
            {
                readAllIndex();
                refushWindow();//刷新窗口
            }
            else //如果文件不存在
            {
                //欢迎
                var welcome = new welcome();
                welcome.ShowDialog();
                this.WindowState = WindowState.Normal;
                this.Show();
            }
        }
        protected override void OnStateChanged(EventArgs e) //隐藏窗口提示事件
        {
            if (WindowState == WindowState.Minimized)
            {
                // 隐藏窗口
                this.Hide();

                // 可选: 显示托盘图标的通知
                notifyIcon.ShowBalloonTip(1000, "应用已最小化", "双击图标打开主页。", System.Windows.Forms.ToolTipIcon.Info);
            }
            base.OnStateChanged(e);
        }
        private void NotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e) //托盘双击事件
        {
            this.ShowInTaskbar = true;
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        public void saveAllIndex()
        {
            File.WriteAllLines(listInfoLocation, items);//保存list信息
        }
        public void readAllIndex()
        {
            try //尝试转换
            {
                //读取
                items = new ObservableCollection<string>(File.ReadAllLines(listInfoLocation).ToList());//转换为列表
                itemsBox.ItemsSource = items;
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
        }
        public void refushWindow()//刷新所有窗口
        {
            // 获取当前应用程序的所有窗口
            var windows = new List<Window>(System.Windows.Application.Current.Windows.Cast<Window>());
            foreach (Window window in windows)
            {
                // 如果当前窗口不是当前窗口，则关闭
                if (window != this)
                {
                    window.Close();
                }
            }
            for (int i = 0; i < items.Count; i++)
            {
                var toolsWindow = new tools(i,items[i]);
                toolsWindow.Show();
            }
        }
        //topBar事件
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new dialog("请选择", "退出还是隐藏", "如果选择确定,程序将退出;如果选择取消,主页将隐藏而小组件能够继续显示");
            bool? canDo =dlg.ShowDialog();
            if(canDo == true)
            {
                // 隐藏托盘图标并释放资源
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                // 关闭应用程序
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                this.ShowInTaskbar = false;
                this.window.Visibility = Visibility.Collapsed;
                this.window.WindowState = WindowState.Minimized;
            }
        }//关闭窗口
        private void topBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove(); //允许拖动窗口
        }//拖拽事件
        //topBar事件
        private void addNewItemButton_Click(object sender, RoutedEventArgs e) //新增小组件
        {
            buttonsView.Visibility = Visibility.Collapsed;//隐藏buttons控件
            seindex = items.Count;
            var setWindow = new setWindow(seindex, "EventName");//创建设置窗口
            bool? canDo = setWindow.ShowDialog();//应用更改变量
            if (canDo == true)//如果允许进行
            {
                if (items != null) //如果列表初始化完毕
                {
                    seindex++;//增加索引
                    items.Add(setWindow.EventName); //增加列表项目
                    saveAllIndex(); //保存列表内容
                    Console.WriteLine(setWindow.EventName);
                    refushWindow();//刷新小组件
                }
            }
        }

        private void changeButton_Click(object sender, RoutedEventArgs e)
        {
            buttonsView.Visibility = Visibility.Collapsed;//隐藏buttons控件
            if (itemsBox.SelectedIndex != -1) //如果不是未选中
            {
                seindex = itemsBox.SelectedIndex; //更改选中索引
                setWindow setWindow;
                if (itemsBox.SelectedItem != null)
                {
                    setWindow = new setWindow(seindex, itemsBox.SelectedItem.ToString());//创建设置窗口
                }
                else
                {
                    setWindow = new setWindow(seindex, "EventName");//创建设置窗口
                }
                bool? canDo = setWindow.ShowDialog();//应用更改变量
                if (canDo == true)//如果允许进行
                {
                    items[seindex] = setWindow.EventName;//更改选中的事件名
                    itemsBox.ItemsSource=items;//绑定关系
                    refushWindow();//刷新小组件
                }
            }
        }
        //listBox追焦事件
        private void itemsBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(items.Count);
            if (itemsBox.SelectedIndex != -1 && seindex < items.Count) //如果不是未选中
            {
                buttonsView.Visibility = Visibility.Visible;//显示button控件
            }
        }
        private void itemsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            seindex = itemsBox.SelectedIndex;
            if (itemsBox.SelectedIndex != -1 && seindex<items.Count) //如果不是未选中
            {
                buttonsView.Visibility = Visibility.Visible;//显示button控件
            }
        }
        private void itemsBox_GotFocus(object sender, RoutedEventArgs e)
        {
            seindex = itemsBox.SelectedIndex;
            if (itemsBox.SelectedIndex != -1 && seindex < items.Count) //如果不是未选中
            {
                buttonsView.Visibility = Visibility.Visible;//显示button控件
            }
        }
        //listBox追焦事件
        private void deleteButton_Click(object sender, RoutedEventArgs e)//删除事件
        {
            buttonsView.Visibility = Visibility.Collapsed;
            toolsInfoLocation = System.Environment.CurrentDirectory + @"\info" + itemsBox.SelectedItem.ToString() + ".inf"; //获取小组件信息地址
            var dialog = new dialog("你确定吗", "警告!", "你确定要删除吗?");
            bool? canDo = dialog.ShowDialog();
            if(canDo == true)
            {
                if (File.Exists(toolsInfoLocation))//如果小组件信息存在
                {
                    try
                    {
                        File.Delete(toolsInfoLocation);//删除配置信息
                        items.RemoveAt(seindex); //删除列表项目
                        saveAllIndex();
                        readAllIndex();
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err);
                    }
                }
                else
                {
                    items.RemoveAt(seindex); //删除列表项目
                    Console.WriteLine(seindex);
                    Console.WriteLine("不存在");
                }
                refushWindow();//刷新小组件
            }
        }

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            var setButton = new MainSetWindow();
            setButton.ShowDialog();
        }
    }
}
