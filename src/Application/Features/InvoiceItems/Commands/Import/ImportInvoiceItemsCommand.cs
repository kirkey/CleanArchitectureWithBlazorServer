#nullable enable
#nullable disable warnings

using CleanArchitecture.Blazor.Application.Features.InvoiceItems.DTOs;
using CleanArchitecture.Blazor.Application.Features.InvoiceItems.Caching;

namespace CleanArchitecture.Blazor.Application.Features.InvoiceItems.Commands.Import;

public class ImportInvoiceItemsCommand(string fileName, byte[] data) : ICacheInvalidatorRequest<Result<int>>
{
    public string FileName { get; set; } = fileName;
    public byte[] Data { get; set; } = data;
    public string CacheKey => InvoiceItemCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => InvoiceItemCacheKey.Tags;
}

public record class CreateInvoiceItemsTemplateCommand : IRequest<Result<byte[]>>
{

}

public class ImportInvoiceItemsCommandHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper,
    IExcelService excelService,
    IStringLocalizer<ImportInvoiceItemsCommandHandler> localizer)
    :
        IRequestHandler<CreateInvoiceItemsTemplateCommand, Result<byte[]>>,
        IRequestHandler<ImportInvoiceItemsCommand, Result<int>>
{
    private readonly InvoiceItemDto _dto = new();

#nullable disable warnings
    public async Task<Result<int>> Handle(ImportInvoiceItemsCommand request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);

        var result = await excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, InvoiceItemDto, object?>>
        {
            { localizer[_dto.GetMemberDescription(x => x.InvoiceId)], (row, item) => item.InvoiceId = row[localizer[_dto.GetMemberDescription(x => x.InvoiceId)]].ToString() },
            { localizer[_dto.GetMemberDescription(x => x.Name)], (row, item) => item.Name = row[localizer[_dto.GetMemberDescription(x => x.Name)]].ToString() },
            { localizer[_dto.GetMemberDescription(x => x.Quantity)], (row, item) => item.Quantity = Convert.ToInt32(row[localizer[_dto.GetMemberDescription(x => x.Quantity)]]) },
            { localizer[_dto.GetMemberDescription(x => x.UnitPrice)], (row, item) => item.UnitPrice = Convert.ToDecimal(row[localizer[_dto.GetMemberDescription(x => x.UnitPrice)]]) },
            { localizer[_dto.GetMemberDescription(x => x.LineTotal)], (row, item) => item.LineTotal = Convert.ToDecimal(row[localizer[_dto.GetMemberDescription(x => x.LineTotal)]]) }
        }, localizer[_dto.GetClassDescription()]);

        if (result.Succeeded && result.Data is not null)
        {
            foreach (var dto in result.Data)
            {
                var exists = await db.InvoiceItems.AnyAsync(x => x.InvoiceId == dto.InvoiceId && x.Name == dto.Name, cancellationToken);
                if (!exists)
                {
                    var item = mapper.Map<InvoiceItem>(dto);
                    // add create domain events if this entity implement the IHasDomainEvent interface
                    // item.AddDomainEvent(new InvoiceItemCreatedEvent(item));
                    await db.InvoiceItems.AddAsync(item, cancellationToken);
                }
            }
            await db.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result.Data.Count());
        }
        else
        {
            return await Result<int>.FailureAsync(result.Errors);
        }
    }

    public async Task<Result<byte[]>> Handle(CreateInvoiceItemsTemplateCommand request, CancellationToken cancellationToken)
    {
        var fields = new string[] {
            localizer[_dto.GetMemberDescription(x => x.InvoiceId)],
            localizer[_dto.GetMemberDescription(x => x.Name)],
            localizer[_dto.GetMemberDescription(x => x.Quantity)],
            localizer[_dto.GetMemberDescription(x => x.UnitPrice)],
            localizer[_dto.GetMemberDescription(x => x.LineTotal)]
        };
        var result = await excelService.CreateTemplateAsync(fields, localizer[_dto.GetClassDescription()]);
        return await Result<byte[]>.SuccessAsync(result);
    }
}
