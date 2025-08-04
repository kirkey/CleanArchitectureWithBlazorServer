#nullable enable
#nullable disable warnings

using CleanArchitecture.Blazor.Application.Features.ChartOfAccounts.DTOs;
using CleanArchitecture.Blazor.Application.Features.ChartOfAccounts.Caching;

namespace CleanArchitecture.Blazor.Application.Features.ChartOfAccounts.Commands.Import;

public class ImportChartOfAccountsCommand(string fileName, byte[] data) : ICacheInvalidatorRequest<Result<int>>
{
    public string FileName { get; set; } = fileName;
    public byte[] Data { get; set; } = data;
    public string CacheKey => ChartOfAccountCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => ChartOfAccountCacheKey.Tags;
}

public record class CreateChartOfAccountsTemplateCommand : IRequest<Result<byte[]>>
{
}

public class ImportChartOfAccountsCommandHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper,
    IExcelService excelService,
    IStringLocalizer<ImportChartOfAccountsCommandHandler> localizer)
    :
        IRequestHandler<CreateChartOfAccountsTemplateCommand, Result<byte[]>>,
        IRequestHandler<ImportChartOfAccountsCommand, Result<int>>
{
    private readonly ChartOfAccountDto _dto = new();

#nullable disable warnings
    public async Task<Result<int>> Handle(ImportChartOfAccountsCommand request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);

        var result = await excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, ChartOfAccountDto, object?>>
        {
            { localizer[_dto.GetMemberDescription(x => x.Name)], (row, item) => item.Name = row[localizer[_dto.GetMemberDescription(x => x.Name)]].ToString() },
            { localizer[_dto.GetMemberDescription(x => x.AccountType)], (row, item) => item.AccountType = Enum.Parse<AccountType>(row[localizer[_dto.GetMemberDescription(x => x.AccountType)]].ToString()) },
            { localizer[_dto.GetMemberDescription(x => x.SubAccountOf)], (row, item) => item.SubAccountOf = row[localizer[_dto.GetMemberDescription(x => x.SubAccountOf)]].ToString() },
            { localizer[_dto.GetMemberDescription(x => x.Description)], (row, item) => item.Description = row[localizer[_dto.GetMemberDescription(x => x.Description)]].ToString() },
            { localizer[_dto.GetMemberDescription(x => x.IsActive)], (row, item) => item.IsActive = Convert.ToBoolean(row[localizer[_dto.GetMemberDescription(x => x.IsActive)]]) }
        }, localizer[_dto.GetClassDescription()]);

        if (result.Succeeded && result.Data is not null)
        {
            foreach (var dto in result.Data)
            {
                var exists = await db.ChartOfAccounts.AnyAsync(x => x.Name == dto.Name, cancellationToken);
                
                if (!exists)
                {
                    var item = mapper.Map<ChartOfAccount>(dto);
                    // add create domain events if this entity implement the IHasDomainEvent interface
                    // item.AddDomainEvent(new ChartOfAccountCreatedEvent(item));
                    await db.ChartOfAccounts.AddAsync(item, cancellationToken);
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

    public async Task<Result<byte[]>> Handle(CreateChartOfAccountsTemplateCommand request, CancellationToken cancellationToken)
    {
        var fields = new string[]
        {
            localizer[_dto.GetMemberDescription(x => x.Name)],
            localizer[_dto.GetMemberDescription(x => x.AccountType)],
            localizer[_dto.GetMemberDescription(x => x.SubAccountOf)],
            localizer[_dto.GetMemberDescription(x => x.Description)],
            localizer[_dto.GetMemberDescription(x => x.IsActive)]
        };
        var result = await excelService.CreateTemplateAsync(fields, localizer[_dto.GetClassDescription()]);
        return await Result<byte[]>.SuccessAsync(result);
    }
}
