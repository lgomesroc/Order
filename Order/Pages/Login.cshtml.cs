using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Order.Services;

namespace Order.Pages;

public class LoginModel(OrderApiClient api) : PageModel
{
    [BindProperty] public string Login { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var result = await api.PostAsync<ApiEnvelope<ApiAuthResponse>>(
                "api/user/auth", new { Login, Password }, "");
            if (string.IsNullOrWhiteSpace(result?.Data?.Token))
            {
                ErrorMessage = "Login ou senha inválidos.";
                return Page();
            }
            HttpContext.Session.SetString("Order.ApiToken", result.Data.Token);
            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
