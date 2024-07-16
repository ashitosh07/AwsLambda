using Amazon.Lambda.Annotations;
using AwsLambda.Application;
using AwsLambda.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AwsLambda.Lambda;

[LambdaStartup]
public class DependencyInjection
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplication();
        services.AddInfrastructure();
    }
}
