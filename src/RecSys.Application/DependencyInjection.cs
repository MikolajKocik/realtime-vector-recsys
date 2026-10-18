using Microsoft.Extensions.DependencyInjection;
using RecSys.Application.Abstractions;
using RecSys.Application.Services;

namespace RecSys.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IVectorMathService, VectorMathService>();

        return services;
    }
}
