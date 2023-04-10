using System.Data.SqlClient;
using System.Data;
using RabbitMQTest.Repository;

namespace RabbitMQTest.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddScoped<IItemRepository, ItemRepository>();    


            return services;
        }
    }
}
