// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.LoginAudits.Specifications;

public enum LoginAuditListView
{
    [Description("All")]
    All,
    [Description("My Login History")]
    My,
    [Description("Created Today")]
    Today,
    [Description("View of the last 30 days")]
    Last30Days,
}

public class LoginAuditAdvancedFilter : PaginationFilter
{
    public LoginAuditListView ListView { get; set; } = LoginAuditListView.Last30Days;
    public UserProfile? CurrentUser { get; set; }
    public bool? Success { get; set; }
    public string? Provider { get; set; }
}
