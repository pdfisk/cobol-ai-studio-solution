using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace BlazorLib.Interop
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Registers BlazorLib interop services.
        /// </summary>
        public static IServiceCollection AddBlazorLibInterop(this IServiceCollection services)
        {
            services.AddScoped<IInteropApi>(sp =>
            {
                var httpClient = new HttpClient { BaseAddress = new Uri(Constants.SimpleHttpServerUrl) };
                return new InteropApi(httpClient);
            });
            return services;
        }
    }
}

