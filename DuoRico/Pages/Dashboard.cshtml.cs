using DuoRico.Data;
using DuoRico.Models;
using DuoRico.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DuoRico.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDropdownService _dropdownService;
    private readonly TransactionService _transactionService;

    public DashboardModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDropdownService dropdownService, TransactionService transactionService)
    {
        _context = context;
        _userManager = userManager;
        _dropdownService = dropdownService;
        _transactionService = transactionService;
    }

    public decimal CurrentMonthIncome { get; set; }
    public decimal CurrentMonthExpense { get; set; }
    public string SelectedMonthName { get; private set; }
    public decimal CurrentMonthBalance => CurrentMonthIncome - CurrentMonthExpense;
    public List<Transaction> Last3Expenses { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int SelectMonth { get; set; }

    [BindProperty(SupportsGet = true)]
    public int SelectYear { get; set; }

    public SelectList MonthOptions { get; set; }
    public SelectList YearOptions { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Filtro de um mês após o atual mês
        if (SelectMonth == 0 || SelectYear == 0)
        {
            var nextMonthDate = DateTime.Now.AddMonths(1);
            SelectMonth = nextMonthDate.Month;
            SelectYear = nextMonthDate.Year;
        }

        var culture = new CultureInfo("pt-BR");
        SelectedMonthName = culture.TextInfo.ToTitleCase(new DateTime(SelectYear, SelectMonth, 1).ToString("MMMM", culture));

        MonthOptions = _dropdownService.GetMonthOptions();
        YearOptions = _dropdownService.GetYearOptions(DateTime.Now.Year, 5);

        var loggedInUser = await _userManager.GetUserAsync(User);
        if (loggedInUser == null || loggedInUser.CoupleId == null) return Challenge();

        // Calcula a soma das receitas e despesas diretamente no banco de dados
        var summary = await _transactionService.GetSummaryForPeriodAsync(loggedInUser.CoupleId.Value, SelectMonth, SelectYear);
        CurrentMonthIncome = summary.TotalIncome;
        CurrentMonthExpense = summary.TotalExpense;

        // Busca as últimas 3 despesas do casal autenticado
        Last3Expenses = await _context.Transactions
            .Where(t => t.Type == TransactionType.Expense && t.Month == SelectMonth && t.Year == SelectYear)
            .OrderBy(t => t.IsPaid)
            .OrderByDescending(t => t.CreatedAt)
            .Take(3)
            .ToListAsync();

        return Page();
    }
}