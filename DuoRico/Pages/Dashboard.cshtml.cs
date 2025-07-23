using DuoRico.Data;
using DuoRico.Models;
using DuoRico.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DuoRico.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDropdownService _dropdownService;

    public DashboardModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDropdownService dropdownService)
    {
        _context = context;
        _userManager = userManager;
        _dropdownService = dropdownService;
    }

    public decimal CurrentMonthIncome { get; set; }
    public decimal CurrentMonthExpense { get; set; }
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
        if (SelectMonth == 0) SelectMonth = DateTime.Now.Month;
        if (SelectYear == 0) SelectYear = DateTime.Now.Year;

        MonthOptions = _dropdownService.GetMonthOptions();
        YearOptions = _dropdownService.GetYearOptions(DateTime.Now.Year, 5);

        var loggedInUser = await _userManager.GetUserAsync(User);
        if (loggedInUser == null || loggedInUser.CoupleId == null) return Challenge();

        var transactions = await _context.Transactions
            .Where(t => t.User.CoupleId == loggedInUser.CoupleId &&
                        t.Month == SelectMonth &&
                        t.Year == SelectYear)
            .ToListAsync();

        CurrentMonthIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        CurrentMonthExpense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

        // Seleciona as 3 últimas despesas
        Last3Expenses = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .OrderByDescending(t => t.CreatedAt)
            .Take(3)
            .ToList();

        return Page();
    }
}