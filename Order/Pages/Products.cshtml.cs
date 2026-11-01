using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Order.Services;

namespace Order.Pages;

public class ProductsModel(OrderApiClient api) : PageModel
{
    public List<ProductViewModel> Products { get; private set; } = [];
    public string? ErrorMessage { get; private set; }
    [BindProperty] public ProductViewModel Form { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (!TryGetToken(out var token)) return RedirectToPage("/Login");
        try { Products = await api.GetAsync<List<ProductViewModel>>("api/product", token) ?? []; }
        catch (Exception ex) { ErrorMessage = ex.Message; }
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!TryGetToken(out var token)) return RedirectToPage("/Login");
        try { await api.PostAsync("api/product", new { Form.Description, Form.SellValue, Form.Stock }, token); TempData["Success"] = "Produto cadastrado."; }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync()
    {
        if (!TryGetToken(out var token)) return RedirectToPage("/Login");
        try { await api.PutAsync($"api/product/{Uri.EscapeDataString(Form.Id)}", Form, token); TempData["Success"] = "Produto atualizado."; }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (!TryGetToken(out var token)) return RedirectToPage("/Login");
        try { await api.DeleteAsync($"api/product/{Uri.EscapeDataString(id)}", token); TempData["Success"] = "Produto excluído."; }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    private bool TryGetToken(out string token)
    {
        token = HttpContext.Session.GetString("Order.ApiToken") ?? "";
        return !string.IsNullOrWhiteSpace(token);
    }
}
