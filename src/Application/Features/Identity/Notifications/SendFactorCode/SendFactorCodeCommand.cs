namespace CleanArchitecture.Blazor.Application.Features.Identity.Notifications.SendFactorCode;

public record SendFactorCodeNotification(string Email, string UserName, string AuthenticatorCode) : INotification;

public class SendFactorCodeNotificationHandler(
    IStringLocalizer<SendFactorCodeNotificationHandler> localizer,
    ILogger<SendFactorCodeNotificationHandler> logger,
    IMailService mailService,
    IApplicationSettings settings)
    : INotificationHandler<SendFactorCodeNotification>
{
    public async Task Handle(SendFactorCodeNotification notification, CancellationToken cancellationToken)
    {
        var subject = localizer["Your Verification Code"];
        var sendMailResult = await mailService.SendAsync(
            notification.Email,
            subject,
            "_authenticatorcode",
            new
            {
                notification.AuthenticatorCode, settings.AppName, notification.Email, notification.UserName,
                settings.Company
            });
        logger.LogInformation("Verification Code email sent to {Email}. Authenticator Code:{AuthenticatorCode} sending result {Successful} {ErrorMessages}",
            notification.Email, notification.AuthenticatorCode,sendMailResult.Successful, string.Join(' ', sendMailResult.ErrorMessages));
    }
}