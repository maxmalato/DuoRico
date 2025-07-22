using DuoRico.Data;
using DuoRico.Models;
using DuoRico.Pages.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DuoRico.Pages_Transactions;

public class IndexModel : TransactionPageModel
{
    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        : base(context, userManager)
    { }

    public IList<Transaction> TransactionList { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string type)
    {
        var (validationResult, loggedInUser) = await ValidateAndLoadContextAsync(type);

        if (validationResult != null) return validationResult;

        TransactionList = await _context.Transactions
            .Include(t => t.User)
            .Where(t => t.User.CoupleId == loggedInUser.CoupleId && t.Type == Type)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return Page();
    }
}