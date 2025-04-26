// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace CleanArchitecture.Blazor.Infrastructure.Services;

public class MailService(
    IApplicationSettings appConfig,
    IFluentEmail fluentEmail,
    ILogger<MailService> logger)
    : IMailService
{
    private const string TemplatePath = "CleanArchitecture.Blazor.Server.UI.Resources.EmailTemplates.{0}.cshtml";
    private readonly IApplicationSettings _appConfig = appConfig;

    public Task<SendResponse> SendAsync(string to, string subject, string body)
    {
        try
        {
            return fluentEmail
                .To(to)
                .Subject(subject)
                .Body(body, true)
                .SendAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to send email. Subject: {EmailSubject}. An exception occurred.", subject);
            throw;
        }
    }

    public Task<SendResponse> SendAsync(string to, string subject, string template, object model)
    {
        try
        {
            return fluentEmail
                .To(to)
                .Subject(subject)
                .UsingTemplateFromEmbedded(string.Format(TemplatePath, template), model, Assembly.GetEntryAssembly())
                .SendAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "Failed to send templated email. Subject: {EmailSubject}, Template: {EmailTemplate}. An exception occurred.",
                subject, template);
            throw;
        }
    }
}