namespace CleanArchitecture.Blazor.Application.Common.Security;

public static partial class Permissions
{
    /// <summary>
    ///     Returns a list of Permissions of Accounting.
    /// </summary>
    /// <returns></returns>

    [DisplayName("Accounting")]
    [Description("Set permissions for Accounting")]
    public static class Accounting
    {
        [Description("Allows viewing accounting data")]
        public const string View = "Permissions.Accounting.View";
        [Description("Allows creating accounting data")]
        public const string Create = "Permissions.Accounting.Create";
        [Description("Allows editing accounting data")]
        public const string Edit = "Permissions.Accounting.Edit";
        [Description("Allows deleting accounting data")]
        public const string Delete = "Permissions.Accounting.Delete";
        [Description("Allows searching accounting data")]
        public const string Search = "Permissions.Accounting.Search";
        [Description("Allows exporting accounting data")]
        public const string Export = "Permissions.Accounting.Export";
    }
}
