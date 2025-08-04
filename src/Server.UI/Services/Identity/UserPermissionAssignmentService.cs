using System.Security.Claims;
using CleanArchitecture.Blazor.Application.Common.ExceptionHandlers;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Security;
using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using ZiggyCreatures.Caching.Fusion;

namespace CleanArchitecture.Blazor.Server.UI.Services.Identity;

/// <inheritdoc />
public class UserPermissionAssignmentService(
    UserManager<ApplicationUser> userManager,
    IPermissionHelper permissionHelper,
    IFusionCache cache,
    ILogger<UserPermissionAssignmentService> logger)
    : IPermissionAssignmentService
{
    private readonly ILogger<UserPermissionAssignmentService> _logger = logger;
    private const string CacheKeyPrefix = "get-claims-by-";

    public async Task<IList<PermissionModel>> LoadAsync(string entityId)
    {
        return await permissionHelper.GetAllPermissionsByUserId(entityId);
    }

    public async Task AssignAsync(PermissionModel model)
    {
        var userId = model.UserId ?? throw new ArgumentNullException(nameof(model.UserId));
        var user = await userManager.FindByIdAsync(userId) ??
                   throw new NotFoundException($"User not found: {userId}");

        var claim = new Claim(model.ClaimType, model.ClaimValue);
        if (model.Assigned)
            await userManager.AddClaimAsync(user, claim);
        else
            await userManager.RemoveClaimAsync(user, claim);

        InvalidateCache(userId);
    }

    public async Task AssignBulkAsync(IEnumerable<PermissionModel> models)
    {
        var list = models.ToList();
        if (!list.Any()) return;

        var userId = list.First().UserId ?? string.Empty;
        var user = await userManager.FindByIdAsync(userId) ??
                   throw new NotFoundException($"User not found: {userId}");

        foreach (var model in list)
        {
            var claim = new Claim(model.ClaimType, model.ClaimValue);
            if (model.Assigned)
                await userManager.AddClaimAsync(user, claim);
            else
                await userManager.RemoveClaimAsync(user, claim);
        }

        InvalidateCache(userId);
    }

    private void InvalidateCache(string userId)
    {
        cache.Remove($"{CacheKeyPrefix}{userId}");
    }
} 