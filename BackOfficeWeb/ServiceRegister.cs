using BackOfficeWeb.Interfaces;
using BackOfficeWeb.Services;

namespace BackOfficeWeb
{
    public static class ServiceRegister
    {
        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            LogWriter.Instance.LogAsync(new ChannelViewModel(), LogType.Trace, "Registering services...");

            // MVC
            builder.Services.AddControllersWithViews();

            // HTTP
            builder.Services.AddHttpClient("MessageBus", httpClient =>
            {
                var messageBusUrl = builder.Configuration.GetSection("Url")["MessageBus"];
                httpClient.BaseAddress = new Uri(messageBusUrl);
            });

            // other microservices
            builder.Services.AddScoped<IMessageBusService, MessageBusService>();

            return builder;
        }
    }
}
