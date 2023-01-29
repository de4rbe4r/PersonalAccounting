using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAccounting.Class
{
    public class Balance
    {
        public float MonthPlan { get; set; }
        public float DayPlan { get; set; }   
        public float MonthBalance { get; set; }
        public float DayBalance { get; set; }
        public DateTime DateRecord { get; set; }
    }
}
