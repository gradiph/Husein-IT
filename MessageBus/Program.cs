using MessageBus;
using MessageBus.APIs;

var app = WebApplication.CreateBuilder(args)
    .RegisterServices()
    .Build();

app.RegisterMiddleware();
app.RegisterSubscriberApi();
app.RegisterChannelApi();
app.RegisterPublishApi();

app.Run();
