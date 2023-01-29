using System;
using System.Data.Entity;
using System.Linq;

namespace PersonalAccounting.Model
{
    public class AccountingContext : DbContext
    {
        public AccountingContext() : base("name=AccountingContext") {      }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().Property(c => c.Name).HasMaxLength(50);
        }

    }
}