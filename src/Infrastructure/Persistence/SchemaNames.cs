namespace CleanArchitecture.Blazor.Infrastructure.Persistence;

public abstract class SchemaNames
{
    public static string Identity { get; set; } = nameof(Identity);
    public static string Sample { get; set; } = nameof(Sample);
    public static string Accounting { get; set; } = nameof(Accounting);
}