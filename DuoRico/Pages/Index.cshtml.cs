using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DuoRico.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        // Verifica se o usuário está auteticado
        if (User.Identity is { IsAuthenticated: true })
        {
            return RedirectToPage("/Dashboard");
        }

        // Se não, redireciona para a página de login
        return RedirectToPage("/Account/Login", new { area = "Identity" });
    }
}