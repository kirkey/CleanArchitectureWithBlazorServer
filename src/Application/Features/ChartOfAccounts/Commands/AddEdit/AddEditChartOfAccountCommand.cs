#nullable enable
#nullable disable warnings

using System.ComponentModel;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Features.ChartOfAccounts.Caching;
using CleanArchitecture.Blazor.Application.Features.ChartOfAccounts.DTOs;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.ChartOfAccounts.Commands.AddEdit;

public class AddEditChartOfAccountCommand : ICacheInvalidatorRequest<Result<string>>
{
    [Description("Id")]
    public string? Id { get; set; }
    [Description("Account Name")]
    public string AccountName { get; set; } = string.Empty;
    [Description("Account Type")]
    public AccountType AccountType { get; set; }
    [Description("Sub Account Of")]
    public string? SubAccountOf { get; set; }
    [Description("Description")]
    public string? Description { get; set; }
    [Description("Is Active")]
    public bool IsActive { get; set; } = true;
    [Description("Name")]
    public string Name { get; set; } = string.Empty;
    [Description("Notes")]
    public string? Notes { get; set; }

    public string CacheKey => ChartOfAccountCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => ChartOfAccountCacheKey.Tags;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ChartOfAccount, AddEditChartOfAccountCommand>(MemberList.None)
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Notes, opt => opt.Ignore());
            CreateMap<AddEditChartOfAccountCommand, ChartOfAccount>(MemberList.None)
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}

public class AddEditChartOfAccountCommandHandler : IRequestHandler<AddEditChartOfAccountCommand, Result<string>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public AddEditChartOfAccountCommandHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<string>> Handle(AddEditChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        if (!string.IsNullOrEmpty(request.Id))
        {
            var entity = await db.ChartOfAccounts.FindAsync(new object[] { request.Id }, cancellationToken);
            if (entity == null)
            {
                return await Result<string>.FailureAsync($"Chart of account with Id '{request.Id}' not found.");
            }
            entity = _mapper.Map(request, entity);
            // entity.AddDomainEvent(new ChartOfAccountUpdatedEvent(entity));
            await db.SaveChangesAsync(cancellationToken);
            return await Result<string>.SuccessAsync(entity.Id);
        }
        else
        {
            var entity = _mapper.Map<ChartOfAccount>(request);
            // entity.AddDomainEvent(new ChartOfAccountCreatedEvent(entity));
            db.ChartOfAccounts.Add(entity);
            await db.SaveChangesAsync(cancellationToken);
            return await Result<string>.SuccessAsync(entity.Id);
        }
    }
}
