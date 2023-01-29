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
    /// Логика взаимодействия для AddCategory.xaml
    /// </summary>
    public partial class AddEditCategory : Window
    {
        bool isEdit = false;
        Category editedCategory;
        public AddEditCategory()
        {
            InitializeComponent();
        }

        public AddEditCategory(Category category)
        {
            InitializeComponent();
            this.Title = "Изменение категории";
            isEdit = true;
            editedCategory = category;
            btnAddCategory.Content = "Изменить категорию";
            tbCategory.Text = editedCategory.Name;
        }


        private void addEditCategory(object sender, RoutedEventArgs e)
        {
            if (tbCategory.Text == "")
            {
                MessageBox.Show("Введите название категории!");
                return;
            }
            if (!isEdit) AddCategory();
            else EditCategory();
        }

        private void AddCategory()
        {
            try
            {
                if (!CommandsClass.AddCategory(tbCategory.Text)) MessageBox.Show($"Категория {tbCategory.Text} уже существует");
                else MessageBox.Show($"Категория {tbCategory.Text} успешно добавлена");
                tbCategory.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении категории. " + ex.Message);
            }
        }

        private void EditCategory()
        {
            try
            {
                if (!CommandsClass.EditCategory(editedCategory, tbCategory.Text)) { MessageBox.Show($"Категория {tbCategory.Text} уже существует"); }
                else
                {
                    MessageBox.Show($"Категория успешно изменена на {tbCategory.Text}");
                    this.Close();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении категории. " + ex.Message);
            }
        }
    }
}
