using PersonalAccounting.Class;
using PersonalAccounting.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static PersonalAccounting.View.MainWindow;

namespace PersonalAccounting.View
{
    /// <summary>
    /// Логика взаимодействия для DayExpenses.xaml
    /// </summary>
    public partial class DayExpenses : Page
    {

        private List<Expense> expenses;
        private readonly PieChart pieChart;
        private Balance balance = CommandsClass.GetBalance();
        public DayExpenses()
        {
            InitializeComponent();
            pieChart = new PieChart(CanvasChart, 150);    // Отрисовка круговой диаграммы
            UpdateInfo();
        }
        private void btn_AddExpenses_Click(object sender, RoutedEventArgs e)
        {
            AddEditExpenses AddEditExpenses = new AddEditExpenses();
            if (AddEditExpenses.FillCategories()) AddEditExpenses.ShowDialog();
            else
            {
                var result = MessageBox.Show("Сначала необходимо добавить категорию, а затем добавлять расход. Перейти в добавление категорий?", "Список категорий пуст!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (result == MessageBoxResult.Yes) btn_AddCategory_Click(sender, e);
            }
            UpdateInfo(); ;
        }

        private void btn_AddCategory_Click(object sender, RoutedEventArgs e)
        {
            AddEditCategory addCategory = new AddEditCategory();
            addCategory.ShowDialog();
        }

        private void GetDayExpenses()
        {
            lvDayExpenses.Items.Clear();
            expenses = CommandsClass.GetDayExpenses(DateTime.Now).OrderBy(e => e.Sum).ToList().OrderByDescending(e => e.Sum).ToList();
            foreach (Expense expense in expenses) lvDayExpenses.Items.Add(expense);
        }

        private void DeleteExpense(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить расход?", "Удаление расхода", MessageBoxButton.YesNo) == MessageBoxResult.No) return; 

            Expense expense = (Expense)lvDayExpenses.SelectedValue;
            MessageBox.Show(CommandsClass.DeleteExpense(expense));  
            UpdateInfo();
        }
        private void EditExpense(object sender, RoutedEventArgs e)
        {
            AddEditExpenses AddEditExpenses = new AddEditExpenses((Expense)lvDayExpenses.SelectedValue);
            AddEditExpenses.ShowDialog();
            UpdateInfo();
        }
        private void GetDaySum()
        {
            float totalSum = 0;
            foreach (Expense expense in lvDayExpenses.Items) totalSum += expense.Sum;
            labelDayExpenses.Content = totalSum.ToString();
        }
        // Обновление диаграммы и таблицы
        private void UpdateInfo()
        {
            GetDayExpenses();
            GetDaySum();
            pieChart.DrawPieChart(CategoryChart.FromExpensesToCategoryChart(expenses), detailsItemsControl);
            PutDayBalance();
            GetDayBalance();
        }
        private void OnOpened(object sender, ContextMenuEventArgs e)
        {
            if (lvDayExpenses.SelectedValue == null)
            {
                itemEdit.IsEnabled = false;
                itemDelete.IsEnabled = false;
            } else
            {
                itemEdit.IsEnabled = true;
                itemDelete.IsEnabled = true;
            }
        }
        private void GetDayBalance()
        {
            if (balance == null) labelDayBalance.Content = "----";
            else labelDayBalance.Content = balance.DayBalance.ToString();
        }
        private void PutDayBalance()
        {
            float daySum = float.Parse(labelDayExpenses.Content.ToString());
            float temp = balance.MonthBalance / (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day) - balance.DayBalance; // необходимо для того, чтобы проверить правильно ли записан остаток на день в файле
            if (daySum != temp)
            {
                balance.DayBalance = balance.DayBalance - (daySum - temp);
                CommandsClass.PutBalance(balance);
            }
        }
    }
    
}
