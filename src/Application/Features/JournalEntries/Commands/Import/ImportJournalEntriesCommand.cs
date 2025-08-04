#nullable enable
#nullable disable warnings

using CleanArchitecture.Blazor.Application.Features.JournalEntries.DTOs;
using CleanArchitecture.Blazor.Application.Features.JournalEntries.Caching;

namespace CleanArchitecture.Blazor.Application.Features.JournalEntries.Commands.Import;

public class ImportJournalEntriesCommand(string fileName, byte[] data) : ICacheInvalidatorRequest<Result<int>>
{
    public string FileName { get; set; } = fileName;
    public byte[] Data { get; set; } = data;
    public string CacheKey => JournalEntryCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => JournalEntryCacheKey.Tags;
}

public record class CreateJournalEntriesTemplateCommand : IRequest<Result<byte[]>>
{

}

public class ImportJournalEntriesCommandHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper,
    IExcelService excelService,
    IStringLocalizer<ImportJournalEntriesCommandHandler> localizer)
    :
        IRequestHandler<CreateJournalEntriesTemplateCommand, Result<byte[]>>,
        IRequestHandler<ImportJournalEntriesCommand, Result<int>>
{
    private readonly JournalEntryDto _dto = new();

#nullable disable warnings
    public async Task<Result<int>> Handle(ImportJournalEntriesCommand request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);

        var result = await excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, JournalEntryDto, object?>>
        {
            { localizer[_dto.GetMemberDescription(x => x.Date)], (row, item) => item.Date = Convert.ToDateTime(row[localizer[_dto.GetMemberDescription(x => x.Date)]]) },
            { localizer[_dto.GetMemberDescription(x => x.ReferenceNumber)], (row, item) => item.ReferenceNumber = row[localizer[_dto.GetMemberDescription(x => x.ReferenceNumber)]].ToString() },
            { localizer[_dto.GetMemberDescription(x => x.Description)], (row, item) => item.Description = row[localizer[_dto.GetMemberDescription(x => x.Description)]].ToString() },
            { localizer[_dto.GetMemberDescription(x => x.Source)], (row, item) => item.Source = row[localizer[_dto.GetMemberDescription(x => x.Source)]].ToString() },
            { localizer[_dto.GetMemberDescription(x => x.IsPosted)], (row, item) => item.IsPosted = Convert.ToBoolean(row[localizer[_dto.GetMemberDescription(x => x.IsPosted)]]) }
        }, localizer[_dto.GetClassDescription()]);

        if (result.Succeeded && result.Data is not null)
        {
            foreach (var dto in result.Data)
            {
                var exists = await db.JournalEntries.AnyAsync(x => x.ReferenceNumber == dto.ReferenceNumber && x.Date == dto.Date, cancellationToken);
                if (!exists)
                {
                    var item = mapper.Map<JournalEntry>(dto);
                    // add create domain events if this entity implement the IHasDomainEvent interface
                    // item.AddDomainEvent(new JournalEntryCreatedEvent(item));
                    await db.JournalEntries.AddAsync(item, cancellationToken);
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

    public async Task<Result<byte[]>> Handle(CreateJournalEntriesTemplateCommand request, CancellationToken cancellationToken)
    {
        var fields = new string[] {
            localizer[_dto.GetMemberDescription(x => x.Date)],
            localizer[_dto.GetMemberDescription(x => x.ReferenceNumber)],
            localizer[_dto.GetMemberDescription(x => x.Description)],
            localizer[_dto.GetMemberDescription(x => x.Source)],
            localizer[_dto.GetMemberDescription(x => x.IsPosted)]
        };
        var result = await excelService.CreateTemplateAsync(fields, localizer[_dto.GetClassDescription()]);
        return await Result<byte[]>.SuccessAsync(result);
    }
}
