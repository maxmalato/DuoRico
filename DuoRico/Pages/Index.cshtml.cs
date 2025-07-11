using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DuoRico.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        // Verifica se o usu�rio est� auteticado
        if (User.Identity is { IsAuthenticated: true })
        {
            return RedirectToPage("/Dashboard");
        }

        // Se n�o, redireciona para a p�gina de login
        return RedirectToPage("/Account/Login", new { area = "Identity" });
    }
}