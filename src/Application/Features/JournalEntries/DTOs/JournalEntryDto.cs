#nullable enable
#nullable disable warnings

namespace CleanArchitecture.Blazor.Application.Features.JournalEntries.DTOs;

public class JournalEntryDto
{
    public DateTime Date { get; set; } = DateTime.Now;
    public string? ReferenceNumber { get; set; }
    public string? Description { get; set; }
    public string? Source { get; set; }
    public bool IsPosted { get; set; } = false;
    [Description("Name")]
    public string Name { get; set; } = string.Empty;
    [Description("Notes")]
    public string? Notes { get; set; }

    public string GetMemberDescription<T>(System.Linq.Expressions.Expression<Func<JournalEntryDto, T>> expression)
    {
        var member = (expression.Body as System.Linq.Expressions.MemberExpression)?.Member;
        return member?.Name ?? string.Empty;
    }
    public string GetClassDescription() => nameof(JournalEntryDto);
}
