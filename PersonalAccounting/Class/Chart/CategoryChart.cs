using PersonalAccounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PersonalAccounting.Class
{
    public class CategoryChart
    {
        private static int _categoryChartCount = 5; // Всего в даиграмме будет отображаться 5 категорий расходов
        public float Sum { get; set; }
        public int Percentage { get; set; }
        public string Title { get; set; }
        public Brush ColorBrush { get; set; }

        // Преобразование из списка расходов в категорию расходов (необходимо для построения круговой диаграммы)
        public static List<CategoryChart> FromExpensesToCategoryChart(List<Expense> expenses)
        {
            List<CategoryChart> categoryToChart = new List<CategoryChart>();
            expenses = expenses.OrderByDescending(e => e.ExpenseCategory.Name).ToList();
            if (expenses.Count == 0) return null;

            int j = 0;
            for (int i = 0; i < expenses.Count; i++)
            {
                if (i != 0 && expenses[i].ExpenseCategory == expenses[i - 1].ExpenseCategory)
                {
                    categoryToChart[j].Sum += expenses[i].Sum;
                }
                else
                {
                    categoryToChart.Add(new CategoryChart
                    {
                        Title = expenses[i].ExpenseCategory.Name,
                        Sum = expenses[i].Sum,
                    });
                }
            }
            return GetTopCategoryChart(categoryToChart);     
        }

        // Сортировка категорий по сумме расходов и возврат первых 4 категорий и пятая категория как "Другие расходы" (сумма мелких категорий расходов)
        public static List<CategoryChart> GetTopCategoryChart(List<CategoryChart> categoryCharts)
        {
            categoryCharts = categoryCharts.OrderByDescending(c => c.Sum).ToList();

            if (categoryCharts.Count > _categoryChartCount)
            {
                for (int i = categoryCharts.Count - 1; i > _categoryChartCount - 1; i--)
                {
                    categoryCharts[_categoryChartCount - 1].Sum = +categoryCharts[i].Sum;
                    categoryCharts.Remove(categoryCharts[i]);
                }
                categoryCharts[_categoryChartCount - 1].Title = "Другие расходы";
            }

            float totalSum = 0;
            foreach (CategoryChart c in categoryCharts)
            {
                totalSum += c.Sum;
            }

            List<SolidColorBrush> colors = new List<SolidColorBrush>()
            {
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4472C4")),
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ED7D31")),
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC000")),
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5B9BD5")),
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A5A5A5"))
            };

            int otherPercentage = 100;

            for (int i = 0; i < _categoryChartCount - 1 && i < categoryCharts.Count - 1; i++)
            {
                categoryCharts[i].ColorBrush = colors[i];
                categoryCharts[i].Percentage = (int)(100 * categoryCharts[i].Sum / totalSum);
                otherPercentage -= categoryCharts[i].Percentage;
            }

            categoryCharts[categoryCharts.Count - 1].ColorBrush = colors[categoryCharts.Count - 1];
            categoryCharts[categoryCharts.Count - 1].Percentage = otherPercentage;
            return categoryCharts;
        }

    }
}
   
