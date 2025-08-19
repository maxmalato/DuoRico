using DuoRico.Data;
using DuoRico.Models;
using DuoRico.Pages.Transactions;
using DuoRico.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DuoRico.Pages_Transactions;

public class IndexModel : TransactionPageModel
{
    private readonly IDropdownService _dropdownService;

    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDropdownService dropdownService)
        : base(context, userManager)
    {
        _dropdownService = dropdownService;
    }

    public IList<Transaction> TransactionList { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public int SelectMonth { get; set; }

    public string SelectedMonthName { get; private set; }

    [BindProperty(SupportsGet = true)]
    public int SelectYear { get; set; }

    public SelectList MonthOptions { get; set; } = default!;
    public SelectList YearOptions { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string type)
    {
        var (validationResult, loggedInUser) = await ValidateAndLoadContextAsync(type);

        if (validationResult != null) return validationResult;

        //Filtro de um mês após o atual mês
        if (SelectMonth == 0 || SelectYear == 0)
        {
            var nextMonthDate = DateTime.Now.AddMonths(1);
            SelectMonth = nextMonthDate.Month;
            SelectYear = nextMonthDate.Year;
        }

        MonthOptions = _dropdownService.GetMonthOptions();
        YearOptions = _dropdownService.GetYearOptions(DateTime.Now.Year, 5);

        var culture = new CultureInfo("pt-BR");
        SelectedMonthName = culture.TextInfo.ToTitleCase(new DateTime(SelectYear, SelectMonth, 1).ToString("MMMM", culture));

        TransactionList = await _context.Transactions
            .Include(t => t.User)
            .Where(
                    t => t.User.CoupleId == loggedInUser.CoupleId &&
                    t.Type == Type &&
                    t.Month == SelectMonth &&
                    t.Year == SelectYear)
            .OrderBy(t => t.IsPaid)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();

        return Page();
    }
}