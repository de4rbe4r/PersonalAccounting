using PersonalAccounting.Class.Model;
using PersonalAccounting.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PersonalAccounting.Class
{
    public static class CommandsClass
    {
        private static AccountingContext context;
        internal static readonly int _categoryChartCount = 5;
        // Подключиться к БД
        public static void ConnectToBD()
        {
            context = new AccountingContext();
        }
        // Получить список расходов за день
        public static List<Expense> GetDayExpenses(DateTime dayDate)
        {
            List<Expense> expenses;
            expenses = (List<Expense>)context.Expenses.Where(e => e.ExpenseDate.Day == dayDate.Day && e.ExpenseDate.Month == dayDate.Month &&
            e.ExpenseDate.Year == dayDate.Year).ToList();
            foreach (Expense expense in expenses) expense.ExpenseCategory = context.Categories.FirstOrDefault(c => c.Id == expense.CategoryId);
            return expenses;
        }
        // Получить список расходов за период
        public static List<DayAndExpense> GetPeriodExpenses(DateTime startDate, DateTime endDate)
        {
            List<DayAndExpense> periodExpenses = new List<DayAndExpense>();
            DateTime currentDate = startDate;
            while (currentDate <= endDate)
            {
                List<Expense> dayExpense = GetDayExpenses(currentDate);
                if (dayExpense.Count != 0)
                {
                    float sum = 0;
                    foreach (Expense expense in dayExpense) sum += expense.Sum;
                    periodExpenses.Add(new DayAndExpense
                    {
                        Day = currentDate,
                        Expenses = dayExpense,
                        TotalSum = sum
                    }); ;
                }
                currentDate = currentDate.AddDays(1);
            }
            return periodExpenses;
        }
        // Преобразование из списка DayAndExpense в список CategoryAndExpense
        public static List<CategoryAndExpense> ConvertFromDAEtoCAE(List<DayAndExpense> dayAndExpenses)
        {
            List<CategoryAndExpense> categoryAndExpenses = new List<CategoryAndExpense>();
            List<Expense> expenses = ConvertFromDAEtoExpenseList(dayAndExpenses);
            int j = 0;

            for (int i = 0; i <= expenses.Count - 1; i++)
            {
                if (i!= 0 && expenses[i].ExpenseCategory.Name == expenses[i - 1].ExpenseCategory.Name)
                {
                    categoryAndExpenses[j - 1].totalSum += expenses[i].Sum;
                    categoryAndExpenses[j - 1].expenses.Add(expenses[i]);
                } else
                {
                    categoryAndExpenses.Add(new CategoryAndExpense
                    {
                        category = expenses[i].ExpenseCategory,
                        totalSum = expenses[i].Sum,
                        expenses = new List<Expense>
                        {
                            expenses[i]
                        }
                    });
                    j++;                  
                }
            }
            return categoryAndExpenses;
        }
        // Получить из списка DayAndExpense полный список Expense
        public static List<Expense> ConvertFromDAEtoExpenseList(List<DayAndExpense> dayAndExpenses)
        {
            List<Expense> expenses = new List<Expense>();
            foreach (DayAndExpense dayAndExpense in dayAndExpenses)
            {
                foreach (Expense exp in dayAndExpense.Expenses) expenses.Add(exp);
            }
            expenses = expenses.OrderBy(e => e.ExpenseCategory.Name).ToList();
            return expenses;
        }
        // Получить список категорий
        public static List<Category> GetCategories()
        {
            return context.Categories.ToList();
        }
        //Добавление новой категории
        public static bool AddCategory(string categoryName)
        {
            if (context.Categories.FirstOrDefault(c => c.Name == categoryName) != null) return false;
            Category newCategory = new Category { Name = categoryName };
            context.Categories.Add(newCategory);
            context.SaveChanges();
            return true;
        }
        //Добавление нового расхода
        public static void AddExpense(string categoryName, string date, float sum)
        {
            Category category = (Category)context.Categories.FirstOrDefault(c => c.Name == categoryName);
            Expense expense = new Expense { 
                CategoryId = category.Id,
                ExpenseDate = DateTime.Parse(date),
                Sum = sum
            };
            context.Expenses.Add(expense);
            context.SaveChanges();
        }
        //Удаление расхода
        public static string DeleteExpense(Expense expense)
        {
            try
            {
                string name = expense.ExpenseCategory.Name;
                context.Expenses.Remove(expense);
                context.SaveChanges();
                return $"Расход ({name} - {expense.Sum}) успешно удален\n";
            } catch (Exception ex)
            {
                return $"Ошибка при удалении - {ex.Message}\n";
            }
        }
        // Удаление списка расходов 
        public static string DeleteListExpenses(List<Expense> expenses)
        {
            string message = "";
            foreach (Expense expense in expenses) message += DeleteExpense(expense);
            return message;
        }

        //Изменение расхода
        public static void EditExpense(Expense expense, string categoryName, string date, float sum)
        {
            Category category = (Category)context.Categories.FirstOrDefault(c => c.Name == categoryName);
            expense.ExpenseCategory = category;
            expense.ExpenseDate = DateTime.Parse(date);
            expense.Sum = sum;
            context.Expenses.AddOrUpdate(expense);
            context.SaveChanges();
        }

        // Изменение категории
        public static bool EditCategory(Category category, string newName)
        {
            if (context.Categories.FirstOrDefault(c => c.Name == newName) != null) return false;
            category.Name = newName;
            context.Categories.AddOrUpdate(category);
            context.SaveChanges();
            return true;
        }
        // Удаление категории
        public static void DeleteCategory(Category category, List<Expense> expenses)
        {
            if (expenses.Count > 0)
            {
                Category tempCategory = context.Categories.FirstOrDefault(c => c.Name == "Без категории");
                if (tempCategory == null) {
                    tempCategory = new Category { Name = "Без категории" };
                    context.Categories.Add(tempCategory);
                    context.SaveChanges();
                    tempCategory = context.Categories.FirstOrDefault(c => c.Name == "Без категории");
                }
                foreach(Expense expense in expenses)
                {
                    EditExpense(expense, "Без категории", expense.ExpenseDate.ToString(), expense.Sum);
                }
            }
            context.Categories.Remove(category);
            context.SaveChanges();
        }
        // Получить список расходов по категории
        public static List<Expense> GetListExpensesByCategory(Category category)
        {
            return context.Expenses.Where(e => e.CategoryId == category.Id).ToList();
        }
        // Получить данные по остатку из файла
        public static Balance GetBalance()
        {
            if (!File.Exists("balance.bin"))           {
                return null;
            }
            Balance balance = new Balance();
            using (BinaryReader reader = new BinaryReader(File.Open("balance.bin", FileMode.Open)))
            {
                balance.DateRecord = DateTime.Parse(reader.ReadString());
                balance.DayPlan = reader.ReadSingle();
                balance.MonthPlan = reader.ReadSingle();
                balance.DayBalance = reader.ReadSingle();
                balance.MonthBalance = reader.ReadSingle();
            }
            if (balance.DateRecord.ToShortDateString() != DateTime.Now.ToShortDateString())
            {
                balance.MonthBalance = balance.MonthBalance - (balance.DayPlan - balance.DayBalance);
                balance.DayBalance = balance.MonthBalance / (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - DateTime.Now.Day);
                balance.DateRecord = DateTime.Now;
            }
            return balance;
        }
        // Положить данные по остатку в файл
        public static void PutBalance(Balance balance)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("balance.bin", FileMode.OpenOrCreate)))
            {
                writer.Write(balance.DateRecord.ToString());
                writer.Write(balance.DayPlan);
                writer.Write(balance.MonthPlan);
                writer.Write(balance.DayBalance);
                writer.Write(balance.MonthBalance);
            }
        }
    }
}
