using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Order.Services;

namespace Order.Pages;

public class OrdersModel(OrderApiClient api) : PageModel
{
    public List<OrderViewModel> Orders { get; private set; } = [];
    public string? ErrorMessage { get; private set; }
    [BindProperty] public OrderForm Form { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (!TryGetToken(out var token)) return RedirectToPage("/Login");
        try { Orders = await api.GetAsync<List<OrderViewModel>>("api/order", token) ?? []; }
        catch (ApiUnauthorizedException)
        {
            HttpContext.Session.Clear();
            TempData["Error"] = "Sua sessão expirou. Faça login novamente.";
            return RedirectToPage("/Login");
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!TryGetToken(out var token)) return RedirectToPage("/Login");
        try
        {
            await api.PostAsync("api/order", new { Form.ClientId, Form.UserId, Items = new[] { new { ProductId = Form.ProductId, Quantity = Form.Quantity } } }, token);
            TempData["Success"] = "Pedido cadastrado.";
        }
        catch (ApiUnauthorizedException)
        {
            HttpContext.Session.Clear();
            TempData["Error"] = "Sua sessão expirou. Faça login novamente.";
            return RedirectToPage("/Login");
        }
        catch (ApiUnauthorizedException)
        {
            HttpContext.Session.Clear();
            TempData["Error"] = "Sua sessão expirou. Faça login novamente.";
            return RedirectToPage("/Login");
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync()
    {
        if (!TryGetToken(out var token)) return RedirectToPage("/Login");
        try
        {
            await api.PutAsync($"api/order/{Uri.EscapeDataString(Form.Id)}", new { Form.Id, Form.ClientId, Form.UserId, Items = new[] { new { ProductId = Form.ProductId, Quantity = Form.Quantity } } }, token);
            TempData["Success"] = "Pedido atualizado.";
        }
        catch (ApiUnauthorizedException)
        {
            HttpContext.Session.Clear();
            TempData["Error"] = "Sua sessão expirou. Faça login novamente.";
            return RedirectToPage("/Login");
        }
        catch (ApiUnauthorizedException)
        {
            HttpContext.Session.Clear();
            TempData["Error"] = "Sua sessão expirou. Faça login novamente.";
            return RedirectToPage("/Login");
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (!TryGetToken(out var token)) return RedirectToPage("/Login");
        try { await api.DeleteAsync($"api/order/{Uri.EscapeDataString(id)}", token); TempData["Success"] = "Pedido excluído."; }
        catch (ApiUnauthorizedException)
        {
            HttpContext.Session.Clear();
            TempData["Error"] = "Sua sessão expirou. Faça login novamente.";
            return RedirectToPage("/Login");
        }
        catch (ApiUnauthorizedException)
        {
            HttpContext.Session.Clear();
            TempData["Error"] = "Sua sessão expirou. Faça login novamente.";
            return RedirectToPage("/Login");
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToPage();
    }

    private bool TryGetToken(out string token)
    {
        token = HttpContext.Session.GetString("Order.ApiToken") ?? "";
        return !string.IsNullOrWhiteSpace(token);
    }
}

public sealed class OrderViewModel
{
    public string Id { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string UserId { get; set; } = "";
    public List<OrderItemViewModel> Items { get; set; } = [];
}
public sealed class OrderItemViewModel
{
    public string ProductId { get; set; } = "";
    public decimal SellValue { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
}
public sealed class OrderForm
{
    public string Id { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string UserId { get; set; } = "";
    public string ProductId { get; set; } = "";
    public int Quantity { get; set; } = 1;
}
