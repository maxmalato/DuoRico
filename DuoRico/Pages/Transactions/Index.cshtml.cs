using DuoRico.Data;
using DuoRico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DuoRico.Pages_Transactions;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IList<Transaction> Transaction { get; set; } = default!;
    public TransactionType Type { get; set; }

    public async Task<IActionResult> OnGetAsync(string type)
    {
        if (!Enum.TryParse<TransactionType>(type, true, out var parsedType))
        {
            return NotFound();
        }

        Type = parsedType;

        var loggedInUser = await _userManager.GetUserAsync(User);

        if (loggedInUser == null)
        {
            Transaction = new List<Transaction>();
            return Page();
        }

        Transaction = await _context.Transactions
            .Include(t => t.User)
            .Where(t => t.User.CoupleId == loggedInUser.CoupleId && t.Type == Type)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return Page();
    }
}