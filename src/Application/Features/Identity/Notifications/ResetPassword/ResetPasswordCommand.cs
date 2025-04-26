namespace CleanArchitecture.Blazor.Application.Features.Identity.Notifications.ResetPassword;

public record ResetPasswordNotification(string RequestUrl, string Email, string UserName) : INotification;

public class ResetPasswordNotificationHandler(
    IStringLocalizer<ResetPasswordNotificationHandler> localizer,
    ILogger<ResetPasswordNotificationHandler> logger,
    IMailService mailService,
    IApplicationSettings settings)
    : INotificationHandler<ResetPasswordNotification>
{
    public async Task Handle(ResetPasswordNotification notification, CancellationToken cancellationToken)
    {
        var sendMailResult = await mailService.SendAsync(
            notification.Email,
            localizer["Verify your recovery email"],
            "_recoverypassword",
            new
            {
                notification.RequestUrl,
                settings.AppName,
                settings.Company,
                notification.UserName,
                notification.Email
            });
        logger.LogInformation(
            "Password rest email sent to {Email}. Reset Password Callback URL: {RequestUrl} sending result {Successful} {ErrorMessages}",
            notification.Email, notification.RequestUrl, sendMailResult.Successful,
            string.Join(' ', sendMailResult.ErrorMessages));
    }
}