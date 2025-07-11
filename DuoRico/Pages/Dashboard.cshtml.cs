using DuoRico.Models;
using DuoRico.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DuoRico.Pages;

[Authorize]
public class DashboardModel(TransactionService transactionService) : PageModel
{
    private readonly TransactionService _transactionService = transactionService;

    public decimal CurrentMonthIncome { get; set; }
    public decimal CurrentMonthExpense { get; set; }
    public decimal CurrentMonthBalance => CurrentMonthIncome - CurrentMonthExpense;
    public List<Transaction> Last3Expenses { get; set; } = new();

    public async Task OnGetAsync(int? month, int? year)
    {
        var selectedMonth = month ?? DateTime.Now.Month;
        var selectedYear = year ?? DateTime.Now.Year;

        // Filtro de mês e ano das transações
        var transactions = await _transactionService.GetCoupleTransactionsForPeriodAsync(selectedMonth, selectedYear);

        CurrentMonthIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        CurrentMonthExpense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        // Seleciona as 3 últimas despesas
        Last3Expenses = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .OrderByDescending(t => t.CreatedAt)
            .Take(3)
            .ToList();
    }
}