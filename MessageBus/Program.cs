using CommonLog;
using MessageBus;
using MessageBus.APIs;
using MessageBus.Models;

var app = WebApplication.CreateBuilder(args)
    .RegisterServices()
    .Build();

app.RegisterMiddleware();
app.RegisterSubscriberApi();
app.RegisterChannelApi();
app.RegisterPublishApi();

app.MapGet("/test", (DataContext db) =>
{
    LogWriter.Instance.LogAsync(new Channel(), LogType.Temp, "Test logging");

    var type = new Channel().GetType().Namespace;
    return type;
});

app.Run();
