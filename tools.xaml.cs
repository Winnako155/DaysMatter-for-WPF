using System;
using System.Collections.Generic;
using System.IO;
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
    /// tools.xaml 的交互逻辑
    /// </summary>
    public partial class tools : Window
    {
        public string myInfoLocation; //小工具地址
        public DateTime today = DateTime.Today; //今天日期
        public int index;//当前小工具序列
        //需要保存的信息
        public string EventName = "EventName"; //当前小工具事件名(默认为"EventName")
        public DateTime Date = DateTime.Today; //当前小工具日期(默认为今天)
        public SolidColorBrush BackGroundColor = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));//默认背景色(白色)
        public SolidColorBrush fontColor = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));//默认字体颜色(黑色)
        public int left = 50; //小工具在桌面上的位置(left)
        public int top = 50; //小工具在桌面上的位置(top)
        public int toolsSize = 0;//小工具的大小模式
        //需要保存的信息
        public tools(int idx, string toolsEventName)
        {
            InitializeComponent();//初始化
            index = idx;
            EventName = toolsEventName;
            myInfoLocation = System.Environment.CurrentDirectory + @"\info" + EventName + ".inf"; //获取配置信息地址
            Console.WriteLine(myInfoLocation);
            if (File.Exists(myInfoLocation))
            {
                //如果文件存在
                readAll();
                applyTheseChange();
            }
            window.Left = left;
            window.Top = top;
        }
        public void saveAll()
        {
            try //尝试保存
            {
                //保存所有文件
                List<string> infos = new List<string>();
                infos.Add("小工具配置信息"); //标识  (0)
                infos.Add(EventName);//保存小工具事件名  (1)
                infos.Add(Date.ToString("d"));//保存日期为短日期(2025/1/1)  (2)
                infos.Add(BackGroundColor.ToString());//保存背景颜色  (3)
                infos.Add(fontColor.ToString());//保存字体颜色  (4)
                infos.Add(left.ToString());//保存小工具在桌面上的位置(left)  (5)
                infos.Add(top.ToString());//保存小工具在桌面上的位置(top)  (6)
                infos.Add(toolsSize.ToString());//保存小工具的大小模式  (7)
                File.WriteAllLines(myInfoLocation, infos); //保存到配置信息中
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
        }
        public void readAll()
        {
            if (File.Exists(myInfoLocation))//判断文件是否存在
            {
                List<string> infos = new List<string>();
                infos = File.ReadAllLines(myInfoLocation).ToList(); //转换为list
                if (infos[0] == "小工具配置信息") //如果标识符正确
                {
                    //读取
                    EventName = infos[1]; //读取小工具事件名  (1)
                    try //尝试转换
                    {
                        Date = DateTime.Parse(infos[2]); //读取日期  (2)
                        BackGroundColor = (SolidColorBrush)new BrushConverter().ConvertFromString(infos[3]); //读取背景颜色  (3)
                        fontColor = (SolidColorBrush)new BrushConverter().ConvertFromString(infos[4]); //读取文字背景颜色  (4)
                        left = int.Parse(infos[5]);  //读取小工具在桌面上的位置(left)  (5)
                        top = int.Parse(infos[6]);  //读取小工具在桌面上的位置(top)  (6)
                        toolsSize = int.Parse(infos[7]);  //读取小工具的大小模式  (7)
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err); //抛出错误信息
                    }
                }
                else
                {
                    Console.WriteLine("不是合法的配置信息");
                }
            }
        }
        public int GetDays(DateTime dayA, DateTime dayB)
        {
            int res = Math.Abs((dayA - dayB).Days); //获取相隔时间(绝对值)
            return (res);
        }
        public void applyTheseChange()
        {
            try
            {
                //应用这些更改
                toolsView.Background = BackGroundColor; //改变小组件背景颜色
                eventName.Foreground = fontColor; //应用字体颜色
                days.Foreground = fontColor;  //应用字体颜色
                tian.Foreground = fontColor;  //应用字体颜色
                eventName.Text = EventName; //应用事件名称
                days.Text = GetDays(Date, today).ToString(); //应用相隔天数
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
        }
        private void DragMove_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            top = (int)window.Top;
            left = (int)window.Left;
            saveAll();
        }//拖拽逻辑
    }
}
