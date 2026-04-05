using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechStore.Application.DTOs;
using TechStore.Application.Services;

namespace TechStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartsController : ControllerBase
{
    private readonly CartService _cartService;

    public CartsController(CartService cartService)
    {
        _cartService = cartService;
    }

    private Guid GetUserId()
    {
        var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(idString) || !Guid.TryParse(idString, out var id))
            throw new UnauthorizedAccessException("Invalid user identity.");
            
        return id;
    }

    [HttpGet("my-cart")]
    public async Task<ActionResult<CartResponse>> GetMyCart()
    {
        var cart = await _cartService.GetCartAsync(GetUserId());
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartResponse>> AddToCart([FromBody] AddToCartRequest request)
    {
        try
        {
            var cart = await _cartService.AddToCartAsync(GetUserId(), request);
            return Ok(cart);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("items/{itemId:guid}")]
    public async Task<ActionResult<CartResponse>> UpdateQuantity(Guid itemId, [FromBody] UpdateCartItemRequest request)
    {
        try
        {
            var cart = await _cartService.UpdateQuantityAsync(GetUserId(), itemId, request);
            return Ok(cart);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Item not found in cart." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("items/{itemId:guid}")]
    public async Task<ActionResult<CartResponse>> RemoveItem(Guid itemId)
    {
        var cart = await _cartService.RemoveItemAsync(GetUserId(), itemId);
        return Ok(cart);
    }

    [HttpDelete("my-cart")]
    public async Task<ActionResult<CartResponse>> ClearCart()
    {
        var cart = await _cartService.ClearCartAsync(GetUserId());
        return Ok(cart);
    }
}
