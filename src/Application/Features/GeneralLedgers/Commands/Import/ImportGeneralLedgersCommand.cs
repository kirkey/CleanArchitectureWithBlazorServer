#nullable enable
#nullable disable warnings

using CleanArchitecture.Blazor.Application.Features.GeneralLedgers.DTOs;
using CleanArchitecture.Blazor.Application.Features.GeneralLedgers.Caching;

namespace CleanArchitecture.Blazor.Application.Features.GeneralLedgers.Commands.Import;

public class ImportGeneralLedgersCommand(string fileName, byte[] data) : ICacheInvalidatorRequest<Result<int>>
{
    public string FileName { get; set; } = fileName;
    public byte[] Data { get; set; } = data;
    public string CacheKey => GeneralLedgerCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => GeneralLedgerCacheKey.Tags;
}

public record class CreateGeneralLedgersTemplateCommand : IRequest<Result<byte[]>>
{

}

public class ImportGeneralLedgersCommandHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper,
    IExcelService excelService,
    IStringLocalizer<ImportGeneralLedgersCommandHandler> localizer)
    :
        IRequestHandler<CreateGeneralLedgersTemplateCommand, Result<byte[]>>,
        IRequestHandler<ImportGeneralLedgersCommand, Result<int>>
{
    private readonly GeneralLedgerDto _dto = new();

#nullable disable warnings
    public async Task<Result<int>> Handle(ImportGeneralLedgersCommand request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);

        var result = await excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, GeneralLedgerDto, object?>>
        {
            { localizer[_dto.GetMemberDescription(x => x.EntryId)], (row, item) => item.EntryId = row[localizer[_dto.GetMemberDescription(x => x.EntryId)]].ToString() },
            { localizer[_dto.GetMemberDescription(x => x.AccountId)], (row, item) => item.AccountId = row[localizer[_dto.GetMemberDescription(x => x.AccountId)]].ToString() },
            { localizer[_dto.GetMemberDescription(x => x.Debit)], (row, item) => item.Debit = Convert.ToDecimal(row[localizer[_dto.GetMemberDescription(x => x.Debit)]]) },
            { localizer[_dto.GetMemberDescription(x => x.Credit)], (row, item) => item.Credit = Convert.ToDecimal(row[localizer[_dto.GetMemberDescription(x => x.Credit)]]) },
            { localizer[_dto.GetMemberDescription(x => x.Memo)], (row, item) => item.Memo = row[localizer[_dto.GetMemberDescription(x => x.Memo)]].ToString() }
        }, localizer[_dto.GetClassDescription()]);

        if (result.Succeeded && result.Data is not null)
        {
            foreach (var dto in result.Data)
            {
                var exists = await db.GeneralLedgers.AnyAsync(x => x.EntryId == dto.EntryId && x.AccountId == dto.AccountId, cancellationToken);
                if (!exists)
                {
                    var item = mapper.Map<GeneralLedger>(dto);
                    // add create domain events if this entity implement the IHasDomainEvent interface
                    // item.AddDomainEvent(new GeneralLedgerCreatedEvent(item));
                    await db.GeneralLedgers.AddAsync(item, cancellationToken);
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

    public async Task<Result<byte[]>> Handle(CreateGeneralLedgersTemplateCommand request, CancellationToken cancellationToken)
    {
        var fields = new string[] {
            localizer[_dto.GetMemberDescription(x => x.EntryId)],
            localizer[_dto.GetMemberDescription(x => x.AccountId)],
            localizer[_dto.GetMemberDescription(x => x.Debit)],
            localizer[_dto.GetMemberDescription(x => x.Credit)],
            localizer[_dto.GetMemberDescription(x => x.Memo)]
        };
        var result = await excelService.CreateTemplateAsync(fields, localizer[_dto.GetClassDescription()]);
        return await Result<byte[]>.SuccessAsync(result);
    }
}
