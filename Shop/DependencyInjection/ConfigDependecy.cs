using Microsoft.Extensions.DependencyInjection;
using Shop.Data;

namespace Shop.DependencyInjection
{
    public static class ConfigDependecy
    {
        public static IServiceCollection AddDependency(this IServiceCollection services)
        {
            services.AddScoped<DataContext, DataContext>();

            return services;
        }
    }
}