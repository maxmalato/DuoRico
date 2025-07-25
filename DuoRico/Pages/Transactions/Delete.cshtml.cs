using DuoRico.Data;
using DuoRico.Helpers;
using DuoRico.Pages.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DuoRico.Pages_Transactions;

public class DeleteModel : TransactionPageModel
{
    public DeleteModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context, userManager)
    {
    }

    public async Task<IActionResult> OnGetAsync(Guid? id, string type)
    {
        if (id == null) return NotFound("Id não encontrado.");

        var (validationResult, loggedInUser) = await ValidateAndLoadContextAsync(type);

        if (validationResult != null) return validationResult;

        Categories = TransactionCategoryHelper.GetCategories(Type);

        Transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.User.CoupleId == loggedInUser.CoupleId);

        if (Transaction == null)
            return NotFound("Transação não encontrada ou você não tem permissão para editá-la.");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid? id, string type)
    {
        if (id == null) return NotFound(0);

        var (validationResult, loggedInUser) = await ValidateAndLoadContextAsync(type);

        if (validationResult != null) return validationResult;

        var transactionToUpdate = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.User.CoupleId == loggedInUser.CoupleId);

        if (transactionToUpdate == null)
            return NotFound("Transação não encontrada ou você não tem permissão para excluí-la.");

        _context.Transactions.Remove(transactionToUpdate);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index", new { type });
    }
}