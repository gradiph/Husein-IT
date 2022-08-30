using Microsoft.EntityFrameworkCore;

namespace MessageBus
{
    public static class ServiceRegister
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            // Database service
            var dockerConnectionString = builder.Configuration.GetConnectionString("Docker");
            builder.Services.AddDbContext<DataContext>(options =>
            options.UseMySql(dockerConnectionString, ServerVersion.AutoDetect(dockerConnectionString))
            );

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            return builder;
        }
    }
}
