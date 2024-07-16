using System.Reflection;
using AwsLambda.Application.Contracts.ServiceInterfaces;
using AwsLambda.Application.Services;
using AwsLambda.Core.Managers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AwsLambda.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddTransient<ISampleAppService, SampleAppService>();
        services.AddTransient<SampleManager>();

        return services;
    }
}
