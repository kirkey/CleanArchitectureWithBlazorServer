// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.Export;

public class ExportProductsQuery : ProductAdvancedFilter, IRequest<Result<byte[]>>
{
    public ExportType ExportType { get; set; }
    public ProductAdvancedSpecification Specification => new(this);
}

public class ExportProductsQueryHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper,
    IExcelService excelService,
    IPdfService pdfService,
    IStringLocalizer<ExportProductsQueryHandler> localizer)
    :
        IRequestHandler<ExportProductsQuery, Result<byte[]>>
{
#nullable disable warnings
    public async Task<Result<byte[]>> Handle(ExportProductsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);
        var data = await db.Products.ApplySpecification(request.Specification)
            .AsNoTracking()
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);


        byte[] result;
        Dictionary<string, Func<ProductDto, object?>> mappers;
        switch (request.ExportType)
        {
            case ExportType.Pdf:
                mappers = new Dictionary<string, Func<ProductDto, object?>>
                {
                    { localizer["Brand Name"], item => item.Brand },
                    { localizer["Product Name"], item => item.Name },
                    { localizer["Description"], item => item.Description },
                    { localizer["Price of unit"], item => item.Price },
                    { localizer["Unit"], item => item.Unit }
                    //{ _localizer["Pictures"], item => string.Join(",",item.Pictures??new string[]{ }) },
                };
                result = await pdfService.ExportAsync(data, mappers, localizer["Products"], true);
                break;
            default:
                mappers = new Dictionary<string, Func<ProductDto, object?>>
                {
                    { localizer["Brand Name"], item => item.Brand },
                    { localizer["Product Name"], item => item.Name },
                    { localizer["Description"], item => item.Description },
                    { localizer["Price of unit"], item => item.Price },
                    { localizer["Unit"], item => item.Unit },
                    { localizer["Pictures"], item => JsonSerializer.Serialize(item.Pictures) }
                };
                result = await excelService.ExportAsync(data, mappers, localizer["Products"]);
                break;
        }

        return await Result<byte[]>.SuccessAsync(result);
    }
}