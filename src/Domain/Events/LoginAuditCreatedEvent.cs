using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Domain.Events;

    public class LoginAuditCreatedEvent(LoginAudit item) : DomainEvent
    {
        public LoginAudit Item { get; } = item;
    }

