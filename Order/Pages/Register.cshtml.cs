using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Order.Services;

namespace Order.Pages;

public class RegisterModel(OrderApiClient api) : PageModel
{
    [BindProperty] public string Name { get; set; } = "";
    [BindProperty] public string Login { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    [BindProperty] public string ConfirmPassword { get; set; } = "";
    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "As senhas não coincidem.";
            return Page();
        }

        try
        {
            await api.PostAsync("api/user", new { Name, Login, Password, ConfirmPassword }, "");
            TempData["Success"] = "Conta criada. Faça login para continuar.";
            return RedirectToPage("/Login");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
