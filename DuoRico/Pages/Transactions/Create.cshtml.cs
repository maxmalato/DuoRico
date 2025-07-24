using DuoRico.Data;
using DuoRico.Helpers;
using DuoRico.Models;
using DuoRico.Pages.Transactions;
using DuoRico.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DuoRico.Pages_Transactions;

public class CreateModel : TransactionPageModel
{
    private readonly IDropdownService _dropdownService;

    public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDropdownService dropdownService)
        : base(context, userManager)
    {
        _dropdownService = dropdownService;
    }

    public SelectList MonthOptions { get; set; }
    public SelectList YearOptions { get; set; }
    public SelectList InstallmentOptions { get; set; }

    public async Task<IActionResult> OnGetAsync(string type)
    {
        var (result, _) = await ValidateAndLoadContextAsync(type);

        Categories = TransactionCategoryHelper.GetCategories(Type);
        MonthOptions = _dropdownService.GetMonthOptions();
        YearOptions = _dropdownService.GetYearOptions(DateTime.Now.Year, 5);
        InstallmentOptions = _dropdownService.GetInstallmentOptions(48);

        return result ?? Page();
    }

    public async Task<IActionResult> OnPostAsync(string type)
    {
        var (validationResult, loggedInUser) = await ValidateAndLoadContextAsync(type);

        if (validationResult != null) return validationResult;

        if (!ModelState.IsValid)
        {
            Categories = TransactionCategoryHelper.GetCategories(Type);
            return Page();
        }

        var transactions = new DateTime(Transaction.Year, Transaction.Month, 1);
        var newTransactions = new List<Transaction>();
        var installmentGroupId = Guid.NewGuid();

        // Loop para criar transações para cada parcela, se necessário
        for (int i = 0; i < Transaction.TotalInstallments; i++)
        {
            var currentInstallmentDate = transactions.AddMonths(i);

            var installment = new Transaction
            {
                Id = Guid.NewGuid(),
                Description = Transaction.Description,
                Amount = Transaction.Amount,
                Category = Transaction.Category,
                Type = Type,
                IsPaid = Transaction.IsPaid,
                TotalInstallments = Transaction.TotalInstallments,
                InstallmentNumber = i + 1,
                InstallmentGroupId = Transaction.TotalInstallments > 1 ? installmentGroupId : null,
                Month = currentInstallmentDate.Month,
                Year = currentInstallmentDate.Year,
                CreatedAt = DateTime.UtcNow,
                UserId = loggedInUser.Id
            };

            newTransactions.Add(installment);
        }

        await _context.Transactions.AddRangeAsync(newTransactions);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index", new { type });
    }
}