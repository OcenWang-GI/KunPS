using System.Reflection;
using GameServer.Controllers;
using GameServer.Controllers.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace GameServer.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all controllers that are not abstract and derive from the Controller base class.
        /// </summary>
        /// <param name="services">The service collection to add the controllers to.</param>
        /// <returns>The service collection with added controllers.</returns>
        public static IServiceCollection AddControllers(this IServiceCollection services)
        {
            var controllerTypes = Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(Controller).IsAssignableFrom(t));

            foreach (var type in controllerTypes)
            {
                services.AddScoped(type);
            }

            return services;
        }

        /// <summary>
        /// Registers all types that have the ChatCommandCategoryAttribute attribute.
        /// </summary>
        /// <param name="services">The service collection to add the command handlers to.</param>
        /// <returns>The service collection with added command handlers.</returns>
        public static IServiceCollection AddCommands(this IServiceCollection services)
        {
            var handlerTypes = Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(t => t.IsClass && t.GetCustomAttribute<ChatCommandCategoryAttribute>() != null);

            foreach (var type in handlerTypes)
            {
                services.AddScoped(type);
            }

            return services;
        }
    }
}