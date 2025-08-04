#nullable disable warnings

using CleanArchitecture.Blazor.Application.Features.ChartOfAccounts.Caching;

namespace CleanArchitecture.Blazor.Application.Features.ChartOfAccounts.Commands.Create;

public class CreateChartOfAccountCommand : ICacheInvalidatorRequest<Result<string>>
{
    [Description("Account Name")]
    public string Name { get; set; } = string.Empty;
    
    [Description("Account Type")]
    public AccountType AccountType { get; set; }
    
    [Description("Sub Account Of")]
    public string? SubAccountOf { get; set; }
    
    [Description("Description")]
    public string? Description { get; set; }
    
    [Description("Is Active")]
    public bool IsActive { get; set; } = true;

    public string CacheKey => ChartOfAccountCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => ChartOfAccountCacheKey.Tags;
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateChartOfAccountCommand, ChartOfAccount>(MemberList.None)
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}

public class CreateChartOfAccountCommandHandler(
    IApplicationDbContextFactory dbContextFactory,
    IMapper mapper)
    : IRequestHandler<CreateChartOfAccountCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        await using var db = await dbContextFactory.CreateAsync(cancellationToken);
        var item = mapper.Map<ChartOfAccount>(request);
        item.AddDomainEvent(new ChartOfAccountCreatedEvent(item));
        db.ChartOfAccounts.Add(item);
        await db.SaveChangesAsync(cancellationToken);
        return await Result<string>.SuccessAsync(item.Id);
    }
}
