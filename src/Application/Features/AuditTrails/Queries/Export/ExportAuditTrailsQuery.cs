// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Queries.Export;

public class ExportAuditTrailsQuery : IRequest<byte[]>
{
    public string Keyword { get; set; } = string.Empty;
    public string OrderBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "Descending";
}

public class ExportAuditTrailsQueryHandler(
    IApplicationDbContext context,
    IExcelService excelService,
    IMapper mapper,
    IStringLocalizer<ExportAuditTrailsQueryHandler> localizer)
    :
        IRequestHandler<ExportAuditTrailsQuery, byte[]>
{
    public async Task<byte[]> Handle(ExportAuditTrailsQuery request, CancellationToken cancellationToken)
    {
        var data = await context.AuditTrails
            .Where(x => x.TableName!.Contains(request.Keyword))
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
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