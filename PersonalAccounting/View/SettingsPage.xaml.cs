using PersonalAccounting.Class;
using PersonalAccounting.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PersonalAccounting.View
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        List<Category> categories;
        Balance balance;
        public SettingsPage()
        {
            InitializeComponent();
            UpdateCategories();
            DisableCategoryButtons();
            GetBalancePlan();
        }

        private void UpdateCategories()
        {
            lvCategories.Items.Clear();
            categories = CommandsClass.GetCategories();
            foreach (Category category in categories) lvCategories.Items.Add(category);
        }

        private void EditCategory(object sender, RoutedEventArgs e)
        {
            Category category = (Category)lvCategories.SelectedValue;
            AddEditCategory editCategory = new AddEditCategory(category);
            editCategory.ShowDialog();
            UpdateCategories();
            lvCategories.SelectedIndex = -1;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvCategories.SelectedIndex == -1) DisableCategoryButtons();
            else EnableCategoryButtons();
        }
        private void DisableCategoryButtons()
        {
            btnEditCategory.IsEnabled = false;
            btnDeleteCategory.IsEnabled = false;
        }
        private void EnableCategoryButtons()
        {
            btnEditCategory.IsEnabled = true;
            btnDeleteCategory.IsEnabled = true;
        }
        private void DeleteCategory(object sender, RoutedEventArgs e)
        {
            string message = "";
            Category category = (Category)lvCategories.SelectedValue;
            List<Expense> expenses = CommandsClass.GetListExpensesByCategory(category);
            if (expenses.Count > 0) message = $"Данная категория используется в ваших расходах. Ее удаление приведет к изменению категории в расходах на \"Без категории\"";

            var result = MessageBox.Show("Вы действительно хотите удалить данную категорию? " + message, "Удаление категории", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                CommandsClass.DeleteCategory(category, expenses);
                UpdateCategories();
            }

        }
        private void GetBalancePlan()
        {
            balance = CommandsClass.GetBalance();
            if (balance == null)
            {
                labelExpensePlanDay.Content = "----";
                labelExpensePlanMonth.Content = "----";
            } else
            {
                labelExpensePlanDay.Content = balance.DayPlan.ToString();
                labelExpensePlanMonth.Content = balance.MonthPlan.ToString();
            }
        }

        private void EditPlanClick(object sender, RoutedEventArgs e)
        {
            EditPlan editPlan = new EditPlan();
            editPlan.ShowDialog();
            GetBalancePlan();
        }
    }
}
