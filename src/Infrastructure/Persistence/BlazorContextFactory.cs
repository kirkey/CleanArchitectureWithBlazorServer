namespace CleanArchitecture.Blazor.Infrastructure.Persistence;

public class BlazorContextFactory<TContext>(IServiceProvider provider) : IDbContextFactory<TContext>
    where TContext : DbContext
{
    public TContext CreateDbContext()
    {
        if (provider == null)
            throw new InvalidOperationException(
                "You must configure an instance of IServiceProvider");

        return ActivatorUtilities.CreateInstance<TContext>(provider);
    }
}