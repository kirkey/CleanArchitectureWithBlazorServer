#nullable enable
#nullable disable warnings

using CleanArchitecture.Blazor.Application.Features.Payments.DTOs;
using CleanArchitecture.Blazor.Application.Features.Payments.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Payments.Commands.Import;

public class ImportPaymentsCommand(string fileName, byte[] data) : ICacheInvalidatorRequest<Result<int>>
{
    public string FileName { get; set; } = fileName;
    public byte[] Data { get; set; } = data;
    public string CacheKey => PaymentCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => PaymentCacheKey.Tags;
}

public record class CreatePaymentsTemplateCommand : IRequest<Result<byte[]>>
{

}

public class ImportPaymentsCommandHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper,
    IExcelService excelService,
    IStringLocalizer<ImportPaymentsCommandHandler> localizer)
    :
        IRequestHandler<CreatePaymentsTemplateCommand, Result<byte[]>>,
        IRequestHandler<ImportPaymentsCommand, Result<int>>
{
    private readonly PaymentDto _dto = new();

#nullable disable warnings
    public async Task<Result<int>> Handle(ImportPaymentsCommand request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);

        var result = await excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, PaymentDto, object?>>
        {
            { localizer[_dto.GetMemberDescription(x => x.CustomerId)], (row, item) => item.CustomerId = row[localizer[_dto.GetMemberDescription(x => x.CustomerId)]].ToString() },
            { localizer[_dto.GetMemberDescription(x => x.PaymentDate)], (row, item) => item.PaymentDate = Convert.ToDateTime(row[localizer[_dto.GetMemberDescription(x => x.PaymentDate)]]) },
            { localizer[_dto.GetMemberDescription(x => x.Amount)], (row, item) => item.Amount = Convert.ToDecimal(row[localizer[_dto.GetMemberDescription(x => x.Amount)]]) },
            { localizer[_dto.GetMemberDescription(x => x.PaymentMethod)], (row, item) => item.PaymentMethod = Enum.Parse<PaymentMethod>(row[localizer[_dto.GetMemberDescription(x => x.PaymentMethod)]].ToString()) },
            { localizer[_dto.GetMemberDescription(x => x.ReferenceNumber)], (row, item) => item.ReferenceNumber = row[localizer[_dto.GetMemberDescription(x => x.ReferenceNumber)]].ToString() }
        }, localizer[_dto.GetClassDescription()]);

        if (result.Succeeded && result.Data is not null)
        {
            foreach (var dto in result.Data)
            {
                var exists = await db.Payments.AnyAsync(x => x.ReferenceNumber == dto.ReferenceNumber && x.CustomerId == dto.CustomerId, cancellationToken);
                if (!exists)
                {
                    var item = mapper.Map<Payment>(dto);
                    // add create domain events if this entity implement the IHasDomainEvent interface
                    // item.AddDomainEvent(new PaymentCreatedEvent(item));
                    await db.Payments.AddAsync(item, cancellationToken);
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

    public async Task<Result<byte[]>> Handle(CreatePaymentsTemplateCommand request, CancellationToken cancellationToken)
    {
        var fields = new string[] {
            localizer[_dto.GetMemberDescription(x => x.CustomerId)],
            localizer[_dto.GetMemberDescription(x => x.PaymentDate)],
            localizer[_dto.GetMemberDescription(x => x.Amount)],
            localizer[_dto.GetMemberDescription(x => x.PaymentMethod)],
            localizer[_dto.GetMemberDescription(x => x.ReferenceNumber)]
        };
        var result = await excelService.CreateTemplateAsync(fields, localizer[_dto.GetClassDescription()]);
        return await Result<byte[]>.SuccessAsync(result);
    }
}
