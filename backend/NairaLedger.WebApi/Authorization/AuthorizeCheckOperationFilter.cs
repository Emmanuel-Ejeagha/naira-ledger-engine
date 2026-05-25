namespace NairaLedger.WebApi.Authorization;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.MethodInfo.DeclaringType!.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                           || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        if (!hasAuthorize) return;

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
               [new OpenApiSecuritySchemeReference("Bearer")] = new List<string>()
            }
        };
    }
}