#nullable enable
#nullable disable warnings

namespace CleanArchitecture.Blazor.Application.Features.JournalEntries.Caching;

public static class JournalEntryCacheKey
{
    public const string GetAllCacheKey = "all-JournalEntries";
    public static string GetPaginationCacheKey(string parameters) => $"JournalEntryCacheKey:JournalEntriesWithPaginationQuery,{parameters}";
    public static string GetExportCacheKey(string parameters) => $"JournalEntryCacheKey:ExportJournalEntriesQuery,{parameters}";
    public static string GetByIdCacheKey(string id) => $"JournalEntryCacheKey:GetByIdCacheKey,{id}";
    public static IEnumerable<string>? Tags => new string[] { "journalentry" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }
}

