using Microsoft.AspNetCore.Mvc;
using PaymentAutomationAPI.Interfaces;
using PaymentAutomationAPI.Models;

namespace PaymentAutomationAPI.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class MerchantSafeController(IMerchantSafeService merchantSafeService) : ControllerBase
{
    [HttpPost("add-card")]
    public async Task<IActionResult> AddCard([FromBody] MerchantSafeRequest request)
    {
        var response = await merchantSafeService.AddCard(request);
        return Ok(response);
    }

    [HttpPost("update-card")]
    public async Task<IActionResult> UpdateCard([FromBody] MerchantSafeRequest request)
    {
        var response = await merchantSafeService.UpdateCard(request);
        return Ok(response);
    }

    [HttpGet("cards/{safeKey}")]
    public async Task<IActionResult> GetCards(string safeKey)
    {
        var response = await merchantSafeService.GetCards(safeKey);
        return Ok(response);
    }

    [HttpPost("disable-cards/{safeKey}")]
    public async Task<IActionResult> DisableCards(string safeKey)
    {
        var response = await merchantSafeService.DisableCards(safeKey);
        return Ok(response);
    }

    [HttpPost("process-payment")]
    public async Task<IActionResult> ProcessPayment([FromBody] MerchantSafeRequest request)
    {
        var response = await merchantSafeService.ProcessPayment(request);
        return Ok(response);
    }
}