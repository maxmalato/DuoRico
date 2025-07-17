using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DuoRico.Data;
using DuoRico.Models;
using Microsoft.AspNetCore.Identity;

namespace DuoRico.Pages_Transactions;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

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
            ModelState.AddModelError(string.Empty, "Você precisa estar em um casal para visualizar transações.");
            return Page();
        }

        Transaction = await _context.Transactions
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id
                && t.User.CoupleId == loggedInUser.CoupleId);

        if (Transaction == null)
            return NotFound("Transação não encontrada ou você não tem permissão para visualizá-la.");

        return Page();
    }
}