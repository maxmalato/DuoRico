using DuoRico.Data;
using DuoRico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DuoRico.Pages.Transactions;

public class TransactionPageModel : PageModel
{
    protected readonly ApplicationDbContext _context;
    protected readonly UserManager<ApplicationUser> _userManager;

    protected TransactionPageModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public Transaction Transaction { get; set; } = default!;
    public TransactionType Type { get; set; }
    public List<string> Categories { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public int ReturnMonth { get; set; }

    [BindProperty(SupportsGet = true)]
    public int ReturnYear { get; set; }

    protected async Task<(IActionResult, ApplicationUser)> ValidateAndLoadContextAsync(string type)
    {
        if (!Enum.TryParse<TransactionType>(type, true, out var parsedType))
        {
            return (NotFound(), null);
        }

        Type = parsedType;

        var loggedInUser = await _userManager.GetUserAsync(User);
        if (loggedInUser == null && loggedInUser?.CoupleId == null)
        {
            return (Challenge(), null);
        }

        return (null, loggedInUser);
    }
}