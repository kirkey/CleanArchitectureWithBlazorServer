// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Queries.Export;

public class ExportAuditTrailsQuery : IRequest<byte[]>
{
    public string Keyword { get; set; } = string.Empty;
    public string OrderBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "Descending";
}

public class ExportAuditTrailsQueryHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper,
    IExcelService excelService,
    IStringLocalizer<ExportAuditTrailsQueryHandler> localizer)
    :
        IRequestHandler<ExportAuditTrailsQuery, byte[]>
{
    public async Task<byte[]> Handle(ExportAuditTrailsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.AuditTrails.Where(x=>x.TableName.Contains(request.Keyword))
            .ProjectTo<AuditTrailDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        var result = await excelService.ExportAsync(data,
            new Dictionary<string, Func<AuditTrailDto, object?>>
            {
                //{ _localizer["Id"], item => item.Id },
                { localizer["Date Time"], item => item.DateTime.ToString("yyyy-MM-dd HH:mm:ss") },
                { localizer["Table Name"], item => item.TableName },
                { localizer["Audit Type"], item => item.AuditType },
                { localizer["Old Values"], item => item.OldValues },
                { localizer["New Values"], item => item.NewValues },
                { localizer["Primary Key"], item => item.PrimaryKey }
            }, localizer["AuditTrails"]
        );
        return result;
    }
}