namespace CleanArchitecture.Blazor.Application.Features.Identity.Notifications.UserActivation;

public record UserActivationNotification(string ActivationUrl, string Email, string UserId, string UserName)
    : INotification;

public class UserActivationNotificationHandler(
    ILogger<UserActivationNotificationHandler> logger,
    IStringLocalizer<UserActivationNotificationHandler> localizer,
    IMailService mailService,
    IApplicationSettings settings)
    : INotificationHandler<UserActivationNotification>
{
    public async Task Handle(UserActivationNotification notification, CancellationToken cancellationToken)
    {
        var sendMailResult = await mailService.SendAsync(
            notification.Email,
            localizer["Account Activation Required"],
            "_useractivation",
            new
            {
                notification.ActivationUrl,
                settings.AppName,
                settings.Company,
                notification.UserName,
                notification.Email
            });
        logger.LogInformation(
            "Activation email sent to {Email}, Activation Callback URL: {ActivationUrl}. sending result {Successful} {Message}, ",
            notification.Email, notification.ActivationUrl, sendMailResult.Successful,
            string.Join(' ', sendMailResult.ErrorMessages));
    }
}