// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authentication.Cookies;

namespace CleanArchitecture.Blazor.Infrastructure.Services;
#nullable disable
public class ConfigureCookieAuthenticationOptions(ITicketStore ticketStore)
    : IPostConfigureOptions<CookieAuthenticationOptions>
{
    public void PostConfigure(string name,
        CookieAuthenticationOptions options)
    {
        options.SessionStore = ticketStore;
    }
}