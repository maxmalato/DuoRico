using DuoRico.Data;
using DuoRico.Helpers;
using DuoRico.Models;
using DuoRico.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DuoRico.Pages_Transactions;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDropdownService _dropdownService;

    public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDropdownService dropdownService)
    {
        _context = context;
        _userManager = userManager;
        _dropdownService = dropdownService;
    }

    [BindProperty]
    public Transaction Transaction { get; set; } = default!;

    public TransactionType Type { get; set; }
    public List<string> Categories { get; set; } = new();
    public SelectList MonthOptions { get; set; }
    public SelectList YearOptions { get; set; }
    public SelectList InstallmentOptions { get; set; }

    public IActionResult OnGet(string type)
    {
        if (!Enum.TryParse<TransactionType>(type, true, out var parsedType))
        {
            return NotFound();
        }

        Type = parsedType;
        Categories = TransactionCategoryHelper.GetCategories(Type);
        MonthOptions = _dropdownService.GetMonthOptions();
        YearOptions = _dropdownService.GetYearOptions(DateTime.Now.Year, 5);
        InstallmentOptions = _dropdownService.GetInstallmentOptions(48);

        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string type)
    {
        if (!Enum.TryParse<TransactionType>(type, true, out var parsedType))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            Type = parsedType;
            Categories = TransactionCategoryHelper.GetCategories(Type);
            return Page();
        }

        var loggedInUser = await _userManager.GetUserAsync(User);

        if (loggedInUser == null && loggedInUser?.CoupleId == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        // Definir propriedades automáticas antes de salvar, ou seja, elas não são passadas pelo formulário
        Transaction.Type = parsedType;
        Transaction.CreatedAt = DateTime.UtcNow;
        Transaction.UserId = loggedInUser.Id;

        _context.Transactions.Add(Transaction);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index", new { type });
    }
}