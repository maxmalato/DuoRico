using DuoRico.Data;
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

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id == null)
            return NotFound();

        var loggedInUser = await _userManager.GetUserAsync(User);

        if (loggedInUser == null)
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        if (loggedInUser.CoupleId == null)
        {
            ModelState.AddModelError(string.Empty, "Voc� precisa estar em um casal para excluir transa��es.");
            return Page();
        }

        Transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id
                && t.User.CoupleId == loggedInUser.CoupleId
                && t.Type == TransactionType.Expense);

        if (Transaction == null)
            return NotFound("Transa��o n�o encontrada ou voc� n�o tem permiss�o para exclu�-la.");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid? id)
    {
        if (id == null)
            return NotFound();

        var loggedInUser = await _userManager.GetUserAsync(User);

        if (loggedInUser == null)
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        if (loggedInUser.CoupleId == null)
        {
            ModelState.AddModelError(string.Empty, "Voc� precisa estar em um casal para excluir transa��es.");
            return Page();
        }

        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id
                && t.User.CoupleId == loggedInUser.CoupleId
                && t.Type == TransactionType.Expense);

        if (transaction == null)
            return NotFound("Transa��o n�o encontrada ou voc� n�o tem permiss�o para exclu�-la.");

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}