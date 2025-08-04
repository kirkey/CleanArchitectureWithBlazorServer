#nullable enable
#nullable disable warnings

namespace CleanArchitecture.Blazor.Application.Features.ChartOfAccounts.Caching;

/// <summary>
/// Static class for managing cache keys and expiration for ChartOfAccount-related data.
/// </summary>
public static class ChartOfAccountCacheKey
{
    public const string GetAllCacheKey = "all-ChartOfAccounts";
    public static string GetPaginationCacheKey(string parameters) {
        return $"ChartOfAccountCacheKey:ChartOfAccountsWithPaginationQuery,{parameters}";
    }
    public static string GetExportCacheKey(string parameters) {
        return $"ChartOfAccountCacheKey:ExportChartOfAccountsQuery,{parameters}";
    }
    public static string GetByIdCacheKey(string id) {
        return $"ChartOfAccountCacheKey:GetByIdCacheKey,{id}";
    }
    public static string GetByNameCacheKey(string name) {
        return $"ChartOfAccountCacheKey:GetByNameCacheKey,{name}";
    }
    public static IEnumerable<string>? Tags => new string[] { "chartofaccount" };

    private static readonly object _tokenLock = new object();
    private static CancellationTokenSource _tokenSource = new CancellationTokenSource();

    public static CancellationToken GetOrCreateToken()
    {
        lock (_tokenLock)
        {
            if (_tokenSource.Token.IsCancellationRequested)
            {
                _tokenSource = new CancellationTokenSource();
            }
            return _tokenSource.Token;
        }
    }

    public static void Refresh()
    {
        lock (_tokenLock)
        {
            if (!_tokenSource.Token.IsCancellationRequested)
            {
                _tokenSource.Cancel();
            }
        }
    }
}
