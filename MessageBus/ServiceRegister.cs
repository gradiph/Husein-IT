using MessageBus.Interfaces;
using MessageBus.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace MessageBus
{
    public static class ServiceRegister
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            // Database services
            var dockerConnectionString = builder.Configuration.GetConnectionString("Docker");
            builder.Services.AddDbContext<DataContext>(options =>
            options.UseMySql(dockerConnectionString, ServerVersion.AutoDetect(dockerConnectionString))
            );

            // HTTP services
            builder.Services.AddHttpClient();

            // API services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Message Bus API",
                    Version = "v1",
                    Description = "Message Bus service for microservices.",
                    TermsOfService = new Uri("http://tempuri.org/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Admin",
                        Email = "admin@tempuri.org"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://www.mit.edu/~amini/LICENSE.md")
                    }
                });
            });

            // Background services
            builder.Services.AddSingleton<IBackgroundMessageQueue>(_ =>
            {
                if (!int.TryParse(builder.Configuration["QueueCapacity"], out var queueCapacity))
                {
                    queueCapacity = 100;
                }
                return new MessageQueueService(queueCapacity);
            });
            builder.Services.AddHostedService<MessageSenderService>();
            builder.Services.AddHostedService<MessageResendService>();

            return builder;
        }
    }
}
