#nullable enable
#nullable disable warnings

using CleanArchitecture.Blazor.Application.Features.BillItems.DTOs;
using CleanArchitecture.Blazor.Application.Features.BillItems.Caching;

namespace CleanArchitecture.Blazor.Application.Features.BillItems.Commands.Import;

public class ImportBillItemsCommand(string fileName, byte[] data) : ICacheInvalidatorRequest<Result<int>>
{
    public string FileName { get; set; } = fileName;
    public byte[] Data { get; set; } = data;
    public string CacheKey => BillItemCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => BillItemCacheKey.Tags;
}

public record class CreateBillItemsTemplateCommand : IRequest<Result<byte[]>>
{

}

public class ImportBillItemsCommandHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper,
    IExcelService excelService,
    IStringLocalizer<ImportBillItemsCommandHandler> localizer)
    :
        IRequestHandler<CreateBillItemsTemplateCommand, Result<byte[]>>,
        IRequestHandler<ImportBillItemsCommand, Result<int>>
{
    private readonly BillItemDto _dto = new();

#nullable disable warnings
    public async Task<Result<int>> Handle(ImportBillItemsCommand request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);

        var result = await excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, BillItemDto, object?>>
        {
            { localizer[_dto.GetMemberDescription(x => x.BillId)], (row, item) => item.BillId = row[localizer[_dto.GetMemberDescription(x => x.BillId)]].ToString() },
            { localizer[_dto.GetMemberDescription(x => x.Name)], (row, item) => item.Name = row[localizer[_dto.GetMemberDescription(x => x.Name)]].ToString() },
            { localizer[_dto.GetMemberDescription(x => x.Quantity)], (row, item) => item.Quantity = Convert.ToInt32(row[localizer[_dto.GetMemberDescription(x => x.Quantity)]]) },
            { localizer[_dto.GetMemberDescription(x => x.UnitPrice)], (row, item) => item.UnitPrice = Convert.ToDecimal(row[localizer[_dto.GetMemberDescription(x => x.UnitPrice)]]) },
            { localizer[_dto.GetMemberDescription(x => x.LineTotal)], (row, item) => item.LineTotal = Convert.ToDecimal(row[localizer[_dto.GetMemberDescription(x => x.LineTotal)]]) }
        }, localizer[_dto.GetClassDescription()]);

        if (result.Succeeded && result.Data is not null)
        {
            foreach (var dto in result.Data)
            {
                var exists = await db.BillItems.AnyAsync(x => x.BillId == dto.BillId && x.Name == dto.Name, cancellationToken);
                if (!exists)
                {
                    var item = mapper.Map<BillItem>(dto);
                    // add create domain events if this entity implement the IHasDomainEvent interface
                    // item.AddDomainEvent(new BillItemCreatedEvent(item));
                    await db.BillItems.AddAsync(item, cancellationToken);
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

    public async Task<Result<byte[]>> Handle(CreateBillItemsTemplateCommand request, CancellationToken cancellationToken)
    {
        var fields = new string[] {
            localizer[_dto.GetMemberDescription(x => x.BillId)],
            localizer[_dto.GetMemberDescription(x => x.Name)],
            localizer[_dto.GetMemberDescription(x => x.Quantity)],
            localizer[_dto.GetMemberDescription(x => x.UnitPrice)],
            localizer[_dto.GetMemberDescription(x => x.LineTotal)]
        };
        var result = await excelService.CreateTemplateAsync(fields, localizer[_dto.GetClassDescription()]);
        return await Result<byte[]>.SuccessAsync(result);
    }
}
