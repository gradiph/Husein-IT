using BackOfficeWeb.Interfaces;
using BackOfficeWeb.Models.MessageBus;
using BackOfficeWeb.Services;

namespace BackOfficeWeb
{
    public static class ServiceRegister
    {
        public const string HTTP_CLIENT_MESSAGE_BUS = "MessageBus";

        public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
        {
            LogWriter.Instance.LogAsync(new ChannelViewModel(), LogType.Trace, "Registering services...");

            // MVC
            builder.Services.AddControllersWithViews();

            // HTTP
            builder.Services.AddHttpClient(HTTP_CLIENT_MESSAGE_BUS, httpClient =>
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
