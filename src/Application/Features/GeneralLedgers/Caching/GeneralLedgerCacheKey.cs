#nullable enable
#nullable disable warnings

namespace CleanArchitecture.Blazor.Application.Features.GeneralLedgers.Caching;

public static class GeneralLedgerCacheKey
{
    public const string GetAllCacheKey = "all-GeneralLedgers";
    public static string GetPaginationCacheKey(string parameters) {
        return $"GeneralLedgerCacheKey:GeneralLedgersWithPaginationQuery,{parameters}";
    }
    public static string GetByEntryIdCacheKey(string entryId) {
        return $"GeneralLedgerCacheKey:GetByEntryIdCacheKey,{entryId}";
    }
    public static string GetByAccountIdCacheKey(string accountId) {
        return $"GeneralLedgerCacheKey:GetByAccountIdCacheKey,{accountId}";
    }
    public static IEnumerable<string>? Tags => new string[] { "generalledger" };

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
