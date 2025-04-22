namespace CleanArchitecture.Blazor.Application.Features.Identity.Notifications.SendWelcome;

public record SendWelcomeNotification(string LoginUrl, string Email, string UserName) : INotification;

public class SendWelcomeNotificationHandler(
    IStringLocalizer<SendWelcomeNotificationHandler> localizer,
    ILogger<SendWelcomeNotificationHandler> logger,
    IMailService mailService,
    IApplicationSettings settings)
    : INotificationHandler<SendWelcomeNotification>
{
    public async Task Handle(SendWelcomeNotification notification, CancellationToken cancellationToken)
    {
        var subject = string.Format(localizer["Welcome to {0}"], settings.AppName);
        var sendMailResult = await mailService.SendAsync(
            notification.Email,
            subject,
            "_welcome",
            new
            {
                notification.LoginUrl, settings.AppName, notification.Email, notification.UserName, settings.Company
            });
        logger.LogInformation("Welcome email sent to {Email}. sending result {Successful} {ErrorMessages}",
            notification.Email, sendMailResult.Successful, string.Join(' ', sendMailResult.ErrorMessages));
    }
}