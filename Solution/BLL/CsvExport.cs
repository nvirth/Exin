using System;
using Common.UiModels.WPF;

namespace BLL
{
    public class ExpenseItemCsv
    {
        public string Title { get; set; }
        public int Amount { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string Category { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }

    public class IncomeItemCsv
    {
        public string Title { get; set; }
        public int Amount { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }

    public static class CsvHelpers
    {
        public static ExpenseItemCsv ToCsv(this ExpenseItem expenseItem)
        {
            return new ExpenseItemCsv {
                Title = expenseItem.Title,
                Amount = expenseItem.Amount,
                Category = expenseItem.Category.Name,
                Unit = expenseItem.Unit.Name,
                Comment = expenseItem.Comment,
                Date = expenseItem.Date,
                Quantity = expenseItem.Quantity,
            };
        }

        public static IncomeItemCsv ToCsv(this IncomeItem incomeItem)
        {
            return new IncomeItemCsv {
                Title = incomeItem.Title,
                Amount = incomeItem.Amount,
                Comment = incomeItem.Comment,
                Date = incomeItem.Date,
            };
        }
    }
}