using Microsoft.EntityFrameworkCore.Query;
using PersonalAccounting.Class;
using PersonalAccounting.Class.Model;
using PersonalAccounting.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace PersonalAccounting.View
{
    /// <summary>
    /// Логика взаимодействия для MonthExpenses.xaml
    /// </summary>
    public partial class MonthExpenses : Page
    {
        private readonly PieChart pieChart;
        List<DayAndExpense> dayAndExpenses;
        public MonthExpenses()
        {
            InitializeComponent();
            labelMonth.Content = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
            pieChart = new PieChart(CanvasChart, 100);    // Отрисовка круговой диаграммы
            UpdateInfo();
        }

        private void GetMonthExpenses()
        {
            lvTotalExpensesInCategory.Items.Clear();
            lvTotalExpensesInDay.Items.Clear();

            dayAndExpenses = CommandsClass.GetPeriodExpenses(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), 
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));
            foreach (DayAndExpense dayAndExpense in dayAndExpenses) lvTotalExpensesInDay.Items.Add(dayAndExpense);

            List<CategoryAndExpense> categoryAndExpenses = CommandsClass.ConvertFromDAEtoCAE(dayAndExpenses).OrderByDescending(e => e.totalSum).ToList();
            foreach (CategoryAndExpense categoryAndExpense in categoryAndExpenses) lvTotalExpensesInCategory.Items.Add(categoryAndExpense);

        }
        private void GetMonthSum()
        {
            float totalSum = 0;
            foreach (CategoryAndExpense me in lvTotalExpensesInCategory.Items) totalSum += me.totalSum;
             labelMonthExpenses.Content = totalSum.ToString();
        }

        private void SelectionChangedTotalExpensesInCategory(object sender, SelectionChangedEventArgs e)
        {
            lvExpensesInCategory.Items.Clear();
            if (lvTotalExpensesInCategory.SelectedIndex != -1) GetExpensesInSelectedCategory();
        }

        private void EditExpense(Expense expense)
        {        
            AddEditExpenses AddEditExpenses = new AddEditExpenses(expense);
            AddEditExpenses.ShowDialog();
            UpdateInfo();
        }

        private void DeleteExpense(Expense expense)
        {
            if (MessageBox.Show("Вы действительно хотите удалить расход?", "Удаление расхода", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            MessageBox.Show(CommandsClass.DeleteExpense(expense));
            UpdateInfo();
        }

        private void DeleteAllExpenses(List<Expense> expenses)
        {
            MessageBox.Show(CommandsClass.DeleteListExpenses(expenses));
            UpdateInfo();
        }


        private void OnOpenedTotalExepensesInDay(object sender, ContextMenuEventArgs e)
        {
            if (lvTotalExpensesInDay.SelectedValue == null) itemDelete_LVTotalExpensesInDay.IsEnabled = false;
            else itemDelete_LVTotalExpensesInDay.IsEnabled = true;
        }

        private void SelectionChangedTotalExpensesInDay(object sender, SelectionChangedEventArgs e)
        {
            lvExpensesInDay.Items.Clear();
            if (lvTotalExpensesInDay.SelectedIndex != -1) GetExpensesInSelectedDay();
        }

        private void GetExpensesInSelectedDay()
        {
            DayAndExpense dayExpenses = (DayAndExpense)lvTotalExpensesInDay.Items[lvTotalExpensesInDay.SelectedIndex];
            dayExpenses.Expenses = dayExpenses.Expenses.OrderByDescending(e => e.Sum).ToList();
            foreach (Expense expense in dayExpenses.Expenses) lvExpensesInDay.Items.Add(expense);
        }
        private void GetExpensesInSelectedCategory()
        {
            CategoryAndExpense categoryExpenses = (CategoryAndExpense)lvTotalExpensesInCategory.Items[lvTotalExpensesInCategory.SelectedIndex];
            categoryExpenses.expenses = categoryExpenses.expenses.OrderByDescending(e => e .Sum).ToList();
            foreach (Expense expense in categoryExpenses.expenses) lvExpensesInCategory.Items.Add(expense);
        }
        private void OnOpenedExpensesInDay(object sender, ContextMenuEventArgs e)
        {
            if (lvExpensesInDay.SelectedValue == null)
            {
                itemEditExpense_LVExpensesInDay.IsEnabled = false;
                itemDeleteExpense_LVExpensesInDay.IsEnabled = false;
            }
            else
            {
                itemEditExpense_LVExpensesInDay.IsEnabled = true;
                itemDeleteExpense_LVExpensesInDay.IsEnabled = true;
            }
        }

        private void OnOpenedTotalExepensesInCategory(object sender, ContextMenuEventArgs e)
        {
            if (lvTotalExpensesInCategory.SelectedValue == null) itemDelete_LVTotalExpensesInCategory.IsEnabled = false;
            else itemDelete_LVTotalExpensesInCategory.IsEnabled = true;
        }

        private void OnOpenedExpensesInCategory(object sender, ContextMenuEventArgs e)
        {
            if (lvExpensesInCategory.SelectedValue == null)
            {
                itemEditExpense_LVExpensesInCategory.IsEnabled = false;
                itemDeleteExpense_LVExpensesInCategory.IsEnabled = false;
            }
            else
            {
                itemEditExpense_LVExpensesInCategory.IsEnabled = true;
                itemDeleteExpense_LVExpensesInCategory.IsEnabled = true;
            }
        }

        private void UpdateInfo()
        {
            GetMonthExpenses();
            GetMonthSum();
            pieChart.DrawPieChart(CategoryChart.FromExpensesToCategoryChart(CommandsClass.ConvertFromDAEtoExpenseList(dayAndExpenses)), detailsItemsControl);
            lvExpensesInCategory.SelectedIndex = -1;
            lvExpensesInDay.SelectedIndex = -1;
        }

        private void DeleteExpenseDate(object sender, RoutedEventArgs e)
        {
            DeleteExpense((Expense)lvExpensesInDay.SelectedValue);
        }

        private void DeleteExpenseCategory(object sender, RoutedEventArgs e)
        {
            DeleteExpense((Expense)lvExpensesInCategory.SelectedValue);
        }

        private void EditExpenseCategory(object sender, RoutedEventArgs e)
        {
            EditExpense((Expense)lvExpensesInCategory.SelectedValue);
        }

        private void EditExpenseDate(object sender, RoutedEventArgs e)
        {
            EditExpense((Expense)lvExpensesInDay.SelectedValue);
        }

        private void DeleteAllCategoryExpense(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить все расходы из данной категории?", "Удаление расходов", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            CategoryAndExpense categoryAndExpense = (CategoryAndExpense)lvTotalExpensesInCategory.SelectedValue;
            DeleteAllExpenses(categoryAndExpense.expenses);

        }

        private void DeleteAllDayExpense(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить все расходы для данного дня?", "Удаление расходов", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            DayAndExpense dayAndExpense = (DayAndExpense)lvTotalExpensesInDay.SelectedValue;
            DeleteAllExpenses(dayAndExpense.Expenses);
        }
    }
}
