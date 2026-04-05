using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechStore.Application.DTOs;
using TechStore.Infrastructure.Shared;

namespace TechStore.Application.Services;

public class UserService
{

    private readonly AppDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserProfileDto?> GetProfileAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return null;

        _logger.LogInformation("Getting profile for user {userId}", userId);
        return new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserProfileDto?> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return null;

        _logger.LogInformation("Updating profile for user {userId}", userId);
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserProfileDto?> FindByUsernameAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync((u => u.FirstName == username));
        
        return new UserProfileDto()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    public static Guid? GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(idClaim, out var id) ? id : null;
    }
}
