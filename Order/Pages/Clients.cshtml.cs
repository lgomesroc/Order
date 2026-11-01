using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Order.Services;

namespace Order.Pages;

public class ClientsModel(OrderApiClient api) : PageModel
{
    public List<ClientViewModel> Clients { get; private set; } = [];
    public string? ErrorMessage { get; private set; }
    [BindProperty] public ClientViewModel Form { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (!TryGetToken(out var token)) return RedirectToPage("/Login");
        try { Clients = await api.GetAsync<List<ClientViewModel>>("api/client", token) ?? []; }
        catch (Exception ex) { ErrorMessage = ex.Message; }
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!TryGetToken(out var token)) return RedirectToPage("/Login");
        try { await api.PostAsync("api/client", new { Form.Name, Form.Email, Form.PhoneNumber, Form.Address }, token); TempData["Success"] = "Cliente cadastrado."; }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync()
    {
        if (!TryGetToken(out var token)) return RedirectToPage("/Login");
        try { await api.PutAsync($"api/client/{Uri.EscapeDataString(Form.Id)}", Form, token); TempData["Success"] = "Cliente atualizado."; }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (!TryGetToken(out var token)) return RedirectToPage("/Login");
        try { await api.DeleteAsync($"api/client/{Uri.EscapeDataString(id)}", token); TempData["Success"] = "Cliente excluído."; }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    private bool TryGetToken(out string token)
    {
        token = HttpContext.Session.GetString("Order.ApiToken") ?? "";
        return !string.IsNullOrWhiteSpace(token);
    }
}

public sealed class ClientViewModel
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Address { get; set; } = "";
}
