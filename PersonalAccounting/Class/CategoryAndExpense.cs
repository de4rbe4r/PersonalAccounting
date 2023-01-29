using PersonalAccounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAccounting.Class.Model
{
    public class CategoryAndExpense
    {
        public Category category { get; set; }
        public List<Expense> expenses { get; set; }
        public float totalSum { get; set; }
    }
}
