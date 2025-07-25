using DuoRico.Data;
using DuoRico.Helpers;
using DuoRico.Models;
using DuoRico.Pages.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DuoRico.Pages_Transactions;

public class EditModel : TransactionPageModel
{
    public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        : base(context, userManager)
    { }

    public async Task<IActionResult> OnGetAsync(Guid? id, string type)
    {
        if (id == null) return NotFound("Id n�o encontrado.");

        var (validationResult, loggedInUser) = await ValidateAndLoadContextAsync(type);

        if (validationResult != null) return validationResult;

        Transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.User.CoupleId == loggedInUser.CoupleId);

        if (Transaction == null)
            return NotFound("Transa��o n�o encontrada ou voc� n�o tem permiss�o para edit�-la.");

        Categories = TransactionCategoryHelper.GetCategories(Type);

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
        {
            return NotFound("Transa��o n�o encontrada ou voc� n�o tem permiss�o para edit�-la.");
        }

        transactionToUpdate.Description = Transaction.Description;
        transactionToUpdate.Amount = Transaction.Amount;
        transactionToUpdate.Category = Transaction.Category;
        transactionToUpdate.IsPaid = Transaction.IsPaid;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TransactionExists(Transaction.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index", new { type });
    }

    private bool TransactionExists(Guid id)
    {
        return _context.Transactions.Any(e => e.Id == id);
    }
}