using DuoRico.Data;
using DuoRico.Helpers;
using DuoRico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DuoRico.Pages_Transactions;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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
        if (!Enum.TryParse<TransactionType>(type, true, out var parsedType))
        {
            return NotFound();
        }

        if (id == null)
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
            ModelState.AddModelError(string.Empty, "Voc� precisa estar em um casal para editar transa��es.");
            return Page();
        }

        Type = parsedType;
        Categories = TransactionCategoryHelper.GetCategories(Type);

        Transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id
                && t.User.CoupleId == loggedInUser.CoupleId
                && t.Type == parsedType);

        if (Transaction == null)
            return NotFound("Transa��o n�o encontrada ou voc� n�o tem permiss�o para edit�-la.");

        return Page();
    }

    /*
    public async Task<IActionResult> OnPostAsync(Guid? id, string type)
    {
        if (!Enum.TryParse<TransactionType>(type, true, out var parsedType))
            return NotFound();

        if (!ModelState.IsValid)
        {
            Type = parsedType;
            Categories = TransactionCategoryHelper.GetCategories(Type);
            return Page();
        }

        var loggedInUser = await _userManager.GetUserAsync(User);

        if (loggedInUser == null)
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        if (loggedInUser.CoupleId == null)
        {
            ModelState.AddModelError(string.Empty, "Voc� precisa estar em um casal para editar transa��es.");
            return Page();
        }

        var transactionToUpdate = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == Transaction.Id
                && t.User.CoupleId == loggedInUser.CoupleId
                && t.Type == parsedType);

        if (transactionToUpdate == null)
            return NotFound("Transa��o n�o encontrada ou voc� n�o tem permiss�o para edit�-la.");

        // Atualiza apenas os campos permitidos
        if (await TryUpdateModelAsync(
            transactionToUpdate,
            "Transaction",
            t => t.Description, t => t.Amount, t => t.Category, t => t.IsPaid))
        {
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index", new { Type });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(Transaction.Id))
                    return NotFound();
                else
                    throw;
            }
        }

        //Type = parsedType;
        //Categories = TransactionCategoryHelper.GetCategories(Type);
        return Page();
    }
    */

    public async Task<IActionResult> OnPostAsync(Guid? id, string type)
    {
        if (!Enum.TryParse<TransactionType>(type, true, out var parsedType))
        {
            return NotFound();
        }

        if (id == null)
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
            ModelState.AddModelError(string.Empty, "Voc� precisa estar em um casal para editar transa��es.");
            return Page();
        }

        var transactionToUpdate = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id
                && t.User.CoupleId == loggedInUser.CoupleId
                && t.Type == parsedType);

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