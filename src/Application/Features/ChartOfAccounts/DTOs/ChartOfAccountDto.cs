#nullable disable warnings

namespace CleanArchitecture.Blazor.Application.Features.ChartOfAccounts.DTOs;

[Description("Chart of Accounts")]
public class ChartOfAccountDto
{
    [Description("Id")]
    public string Id { get; set; } = string.Empty;
    
    [Description("Account Name")]
    public string? Name { get; set; }
    
    [Description("Account Type")]
    public AccountType AccountType { get; set; }
    
    [Description("Sub Account Of")]
    public string? SubAccountOf { get; set; }
    
    [Description("Description")]
    public string? Description { get; set; }
    
    [Description("Is Active")]
    public bool IsActive { get; set; }
    
    [Description("Created")]
    public DateTime? Created { get; set; }
    
    [Description("Created By")]
    public string? CreatedBy { get; set; }
    
    [Description("Last Modified")]
    public DateTime? LastModified { get; set; }
    
    [Description("Last Modified By")]
    public string? LastModifiedBy { get; set; }

    [Description("Notes")]
    public string? Notes { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ChartOfAccount, ChartOfAccountDto>(MemberList.None);
            CreateMap<ChartOfAccountDto, ChartOfAccount>(MemberList.None);
        }
    }
}
