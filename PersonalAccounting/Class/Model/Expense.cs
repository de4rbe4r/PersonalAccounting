using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAccounting.Model
{
    public class Expense
    {
        public int Id { get; set; }
        [Required]
        public float Sum { get; set; }
        public DateTime ExpenseDate { get; set; }
        public int CategoryId { get; set; }
        public Category ExpenseCategory { get; set; }
    }
}
