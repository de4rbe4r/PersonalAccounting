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
using System.Windows.Shapes;

namespace PersonalAccounting.View
{
    /// <summary>
    /// Логика взаимодействия для AddEditExpenses.xaml
    /// </summary>
    public partial class AddEditExpenses : Window
    {
        bool isEdit = false;
        Expense editedExpense;
        public AddEditExpenses()
        {
            InitializeComponent();
            dpDate.Text = DateTime.Now.ToShortDateString();
            dpDate.DisplayDateEnd = DateTime.Now;
        }
        public AddEditExpenses(Expense expense)
        {
            InitializeComponent();
            FillCategories();
            editedExpense = expense;
            this.Title = "Изменение расхода";
            cbCategories.SelectedValue = expense.ExpenseCategory.Name;
            dpDate.Text = expense.ExpenseDate.ToShortDateString();
            dpDate.DisplayDateEnd = DateTime.Now;
            tbSum.Text = expense.Sum.ToString();
            isEdit = true;
            btnAddEdit.Content = "Изменить расход";
        }

        public bool FillCategories()
        {
            List<Category> categories = CommandsClass.GetCategories();
            if (categories.Count == 0) return false;
            foreach (Category c in categories) cbCategories.Items.Add(c.Name);
            return true;
        }

        private void AddEditExpense(object sender, RoutedEventArgs e)
        {
            if (!isEdit) AddExpense();
            else EditExpense();
        }

        private void AddExpense()
        {
            if (!CheckInputs()) return;
            try
            {
                CommandsClass.AddExpense(cbCategories.SelectedValue.ToString(), dpDate.Text, float.Parse(tbSum.Text.Replace('.', ',')));
                tbSum.Text = "";
                MessageBox.Show($"Расход успешно добавлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"При добавлении расхода произошла ошибка: {ex.Message}");
            }
        }
        private void EditExpense()
        {
            if (!CheckInputs()) return;
            try
            {
                CommandsClass.EditExpense(editedExpense, cbCategories.SelectedValue.ToString(), dpDate.Text, float.Parse(tbSum.Text.Replace('.', ',')));
                MessageBox.Show($"Расход успешно изменен!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"При добавлении расхода произошла ошибка: {ex.Message}");
            }
        }

        private bool CheckInputs()
        {
            float temp;
            if (tbSum.Text == "" || float.TryParse(tbSum.Text.Replace('.', ','), out temp) == false) {
                MessageBox.Show("Ошибка ввода расхода!");
                return false;
            }
            if (cbCategories.SelectedValue == null) {
                MessageBox.Show("Не выбрана категория!");
                return false;
            }
            return true;
        }
    }
}
