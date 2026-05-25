namespace NairaLedger.WebApi.Authorization;

public static class Policies
{
    public const string CanManageUsers = "CanManageUsers";
    public const string CanApproveKyc = "CanApproveKyc";
    public const string CanReverseTransaction = "CanReverseTransaction";
    public const string CanViewAuditLogs = "CanViewAuditLogs";
    public const string CanExportStatements = "CanExportStatements";

    public static void Configure(AuthorizationOptions options)
    {
        options.AddPolicy(CanManageUsers, policy => policy.RequireRole("Admin"));
        options.AddPolicy(CanApproveKyc, policy => policy.RequireRole("Admin"));
        options.AddPolicy(CanReverseTransaction, policy => policy.RequireRole("Admin"));
        options.AddPolicy(CanViewAuditLogs, policy => policy.RequireRole("Admin"));
        options.AddPolicy(CanExportStatements, policy => policy.RequireRole("Admin"));
    }
}