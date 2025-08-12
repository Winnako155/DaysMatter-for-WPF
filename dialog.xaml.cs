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
    /// dialog.xaml 的交互逻辑
    /// </summary>
    public partial class dialog : Window
    {
        public dialog(string title,string title1,string content)
        {
            InitializeComponent();
            titleText.Text = title;
            FatherTitle.Text = title1;
            contentText.Text = content;
        }

        private void topBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
