// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Common.Models;

public class PaginatedData<T>(IEnumerable<T> items, int total, int pageIndex, int pageSize)
{
    public int CurrentPage { get; } = pageIndex;
    public int TotalItems { get; private set; } = total;
    public int TotalPages { get; } = (int)Math.Ceiling(total / (double)pageSize);
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public IEnumerable<T> Items { get; set; } = items;

    public static async Task<PaginatedData<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedData<T>(items, count, pageIndex, pageSize);
    }
}