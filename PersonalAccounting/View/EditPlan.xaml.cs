using PersonalAccounting.Class;
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

namespace PersonalAccounting.View
{
    /// <summary>
    /// Логика взаимодействия для EditPlan.xaml
    /// </summary>
    public partial class EditPlan : Window
    {
        public EditPlan()
        {
            InitializeComponent();
            tbDay.IsEnabled = false;
            tbMonth.IsEnabled = false;
        }

        private void CheckRadioButton(object sender, RoutedEventArgs e)
        {
            if (rbDay.IsChecked == true)
            {
                tbDay.IsEnabled = true;
                tbMonth.IsEnabled = false;
            } else if (rbMonth.IsChecked == true)
            {
                tbDay.IsEnabled = false;
                tbMonth.IsEnabled = true;
            }
        }
        private bool CheckInputs()
        {
            float temp;
            if (float.TryParse(tbDay.Text.Replace('.', ','), out temp) == false && tbDay.Text != "")
            {
                tbDay.Background = Brushes.Red;
                return false;
            }
            else if (float.TryParse(tbMonth.Text.Replace('.', ','), out temp) == false && tbMonth.Text != "")
            {
                tbMonth.Background = Brushes.Red;
                return false;
            }
            else
            {
                tbDay.Background = Brushes.White;
                tbMonth.Background = Brushes.White;
                return true;
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!CheckInputs()) return;
            if (tbDay.IsEnabled) tbMonth.Text = ((DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day) * float.Parse(tbDay.Text.Replace('.', ','))).ToString();
            else tbDay.Text = (float.Parse(tbMonth.Text.Replace('.', ',')) / (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day)).ToString();
        }

        private void EditPlanClick(object sender, RoutedEventArgs e)
        {
            if (!CheckInputs()) 
            {
                MessageBox.Show("Проверьте введенные данные!");
                return;
            }
            Balance balance = new Balance();
            balance.MonthPlan = float.Parse(tbMonth.Text.Replace('.', ','));
            balance.DayPlan = float.Parse(tbDay.Text.Replace('.', ','));
            balance.DateRecord = DateTime.Now;
            balance.MonthBalance = balance.MonthPlan;
            balance.DayBalance = balance.MonthBalance / (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day);
            CommandsClass.PutBalance(balance);
            MessageBox.Show("Данные о плане обновлены!");
            this.Close();
        }
    }
}
