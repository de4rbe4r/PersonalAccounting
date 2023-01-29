using Microsoft.EntityFrameworkCore.Diagnostics;
using PersonalAccounting.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAccounting.Class.Model
{
    public class DayAndExpense
    {
        public DateTime Day { get; set; }
        public List<Expense> Expenses { get; set; }
        public float TotalSum { get; set; } 
    }
}
