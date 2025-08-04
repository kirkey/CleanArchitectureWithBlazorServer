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
public class RolePermissionAssignmentService(
    RoleManager<ApplicationRole> roleManager,
    IPermissionHelper permissionHelper,
    IFusionCache cache,
    ILogger<RolePermissionAssignmentService> logger)
    : IPermissionAssignmentService
{
    private readonly ILogger<RolePermissionAssignmentService> _logger = logger;
    private const string CacheKeyPrefix = "get-claims-by-";

    public async Task<IList<PermissionModel>> LoadAsync(string entityId)
    {
        return await permissionHelper.GetAllPermissionsByRoleId(entityId);
    }

    public async Task AssignAsync(PermissionModel model)
    {
        var roleId = model.RoleId ?? throw new ArgumentNullException(nameof(model.RoleId));
        var role = await roleManager.FindByIdAsync(roleId) ??
                   throw new NotFoundException($"Role not found: {roleId}");
        var claim = new Claim(model.ClaimType, model.ClaimValue);
        if (model.Assigned)
            await roleManager.AddClaimAsync(role, claim);
        else
            await roleManager.RemoveClaimAsync(role, claim);
        InvalidateCache(roleId);
    }

    public async Task AssignBulkAsync(IEnumerable<PermissionModel> models)
    {
        var list = models.ToList();
        if (!list.Any()) return;
        var roleId = list.First().RoleId ?? string.Empty;
        var role = await roleManager.FindByIdAsync(roleId) ??
                   throw new NotFoundException($"Role not found: {roleId}");
        foreach (var model in list)
        {
            var claim = new Claim(model.ClaimType, model.ClaimValue);
            if (model.Assigned)
                await roleManager.AddClaimAsync(role, claim);
            else
                await roleManager.RemoveClaimAsync(role, claim);
        }
        InvalidateCache(roleId);
    }

    private void InvalidateCache(string roleId)
    {
        cache.Remove($"{CacheKeyPrefix}{roleId}");
    }
} 