using DuoRico.Data;
using DuoRico.Helpers;
using DuoRico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DuoRico.Pages_Transactions;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public Transaction Transaction { get; set; } = default!;

    public TransactionType Type { get; set; }

    public List<string> Categories { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid? id, string type)
    {
        if (!Enum.TryParse<TransactionType>(type, true, out var parsedType) && id == null)
        {
            return NotFound();
        }

        var loggedInUser = await _userManager.GetUserAsync(User);

        if (loggedInUser == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        if (loggedInUser.CoupleId == null)
        {
            ModelState.AddModelError(string.Empty, "Você precisa estar em um casal para editar transações.");
            return Page();
        }

        Type = parsedType;
        Categories = TransactionCategoryHelper.GetCategories(Type);

        Transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id
                && t.User.CoupleId == loggedInUser.CoupleId
                && t.Type == parsedType);

        if (Transaction == null)
            return NotFound("Transação não encontrada ou você não tem permissão para editá-la.");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid? id, string type)
    {
        if (!Enum.TryParse<TransactionType>(type, true, out var parsedType) && id == null)
        {
            return NotFound();
        }

        var loggedInUser = await _userManager.GetUserAsync(User);

        if (loggedInUser == null)
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        if (loggedInUser.CoupleId == null)
        {
            ModelState.AddModelError(string.Empty, "Você precisa estar em um casal para excluir transações.");
            return Page();
        }

        var transactionToUpdate = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id
                && t.User.CoupleId == loggedInUser.CoupleId
                && t.Type == parsedType);

        if (transactionToUpdate == null)
            return NotFound("Transação não encontrada ou você não tem permissão para excluí-la.");

        _context.Transactions.Remove(transactionToUpdate);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index", new { type });
    }
}