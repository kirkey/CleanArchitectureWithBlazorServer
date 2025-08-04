// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.PicklistSets.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Queries.Export;

public class ExportPicklistSetsQuery : IRequest<byte[]>
{
    public string? Keyword { get; set; }
    public string OrderBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "desc";
}

public class ExportPicklistSetsQueryHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper,
    IExcelService excelService,
    IStringLocalizer<ExportPicklistSetsQueryHandler> localizer)
    :
        IRequestHandler<ExportPicklistSetsQuery, byte[]>
{
#pragma warning disable CS8602
#pragma warning disable CS8604
    public async Task<byte[]> Handle(ExportPicklistSetsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.PicklistSets.Where(x =>
                (string.IsNullOrEmpty(request.Keyword) || x.Value.Contains(request.Keyword) || x.Text.Contains(request.Keyword)))
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectTo<PicklistSetDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        var result = await excelService.ExportAsync(data,
            new Dictionary<string, Func<PicklistSetDto, object?>>
            {
                //{ _localizer["Id"], item => item.Id },
                { localizer["Name"], item => item.Name },
                { localizer["Value"], item => item.Value },
                { localizer["Text"], item => item.Text },
                { localizer["Description"], item => item.Description }
            }, localizer["Data"]
        );
        return result;
    }
}