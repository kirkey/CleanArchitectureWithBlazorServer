// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;


namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.Import;

public class ImportProductsCommand(string fileName, byte[] data) : ICacheInvalidatorRequest<Result<int>>
{
    public string FileName { get; } = fileName;
    public byte[] Data { get; } = data;
    public string CacheKey => ProductCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => ProductCacheKey.Tags;
}

public record CreateProductsTemplateCommand : IRequest<Result<byte[]>>
{
}

public class ImportProductsCommandHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper,
    IExcelService excelService,
    IStringLocalizer<ImportProductsCommandHandler> localizer)
    :
        IRequestHandler<CreateProductsTemplateCommand, Result<byte[]>>,
        IRequestHandler<ImportProductsCommand, Result<int>>
{
    public async Task<Result<byte[]>> Handle(CreateProductsTemplateCommand request, CancellationToken cancellationToken)
    {
        var fields = new string[]
        {
            localizer["Brand Name"],
            localizer["Product Name"],
            localizer["Description"],
            localizer["Unit"],
            localizer["Price of unit"],
            localizer["Pictures"]
        };
        var result = await excelService.CreateTemplateAsync(fields, localizer["Products"]);
        return await Result<byte[]>.SuccessAsync(result);
    }
#nullable disable warnings
    public async Task<Result<int>> Handle(ImportProductsCommand request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);
        var result = await excelService.ImportAsync(request.Data,
            new Dictionary<string, Func<DataRow, ProductDto, object?>>
            {
                { localizer["Brand Name"], (row, item) => item.Brand = row[localizer["Brand Name"]].ToString() },
                { localizer["Product Name"], (row, item) => item.Name = row[localizer["Product Name"]].ToString() },
                {
                    localizer["Description"],
                    (row, item) => item.Description = row[localizer["Description"]].ToString()
                },
                { localizer["Unit"], (row, item) => item.Unit = row[localizer["Unit"]].ToString() },
                {
                    localizer["Price of unit"],
                    (row, item) => item.Price = row.FieldDecimalOrDefault(localizer["Price of unit"])
                },
                {
                    localizer["Pictures"],
                    (row, item) => item.Pictures = string.IsNullOrEmpty(row[localizer["Pictures"]].ToString())
                        ? new List<ProductImage>()
                        : JsonSerializer.Deserialize<List<ProductImage>>(row[localizer["Pictures"]].ToString())
                }
            }, localizer["Products"]);
        if (!result.Succeeded) return await Result<int>.FailureAsync(result.Errors);
        {
            foreach (var dto in result.Data!)
            {
                var item = mapper.Map<Product>(dto);
                await db.Products.AddAsync(item, cancellationToken);
            }

            await db.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result.Data.Count());
        }
    }
}