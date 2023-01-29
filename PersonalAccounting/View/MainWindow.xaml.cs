using PersonalAccounting.Class;
using PersonalAccounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace PersonalAccounting.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Thread thread = new Thread(() => { CommandsClass.ConnectToBD(); CommandsClass.GetDayExpenses(DateTime.Now); });
            MessageWindows message = new MessageWindows();
            message.Show();
            thread.Start();
            InitializeComponent();
            while (true) {
                if (message.Title == "Подключение к базе данных.....") message.Title = "Подключение к базе данных.";
                else message.Title += ".";
                if (!thread.IsAlive)
                {
                    message.Close();
                    break;
                }
                Thread.Sleep(100);
            }
        }
    }
}
