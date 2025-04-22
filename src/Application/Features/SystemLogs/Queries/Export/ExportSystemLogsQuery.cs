// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Features.SystemLogs.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Loggers.Queries.Export;

public class ExportSystemLogsQuery : IRequest<byte[]>
{
    public string Keyword { get; set; } = string.Empty;
    public string OrderBy { get; set; } = "Id";
    public string SortDirection { get; set; } = "Descending";
}

public class ExportSystemLogsQueryHandler(
    IMapper mapper,
    IApplicationDbContext context,
    IExcelService excelService,
    IStringLocalizer<ExportSystemLogsQueryHandler> localizer)
    : IRequestHandler<ExportSystemLogsQuery, byte[]>
{
    public async Task<byte[]> Handle(ExportSystemLogsQuery request, CancellationToken cancellationToken)
    {
        var data = await context.SystemLogs
            .Where(x => x.Message!.Contains(request.Keyword) || x.Exception!.Contains(request.Keyword))
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectTo<SystemLogDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        var result = await excelService.ExportAsync(data,
            new Dictionary<string, Func<SystemLogDto, object?>>
            {
                //{ _localizer["Id"], item => item.Id },
                { localizer["Time Stamp"], item => item.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss") },
                { localizer["Level"], item => item.Level },
                { localizer["Message"], item => item.Message },
                { localizer["Exception"], item => item.Exception },
                { localizer["User Name"], item => item.UserName },
                { localizer["Message Template"], item => item.MessageTemplate },
                { localizer["Properties"], item => item.Properties }
            }, localizer["Logs"]
        );
        return result;
    }
}