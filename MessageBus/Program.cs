global using CommonJson;
global using CommonHttp;
global using CommonLog;
global using MessageBus.Models;
using MessageBus.APIs;
using MessageBus;

LogWriter.Instance.LogAsync(new Channel(), LogType.Trace, "System is starting...");

var app = WebApplication.CreateBuilder(args)
    .RegisterServices()
    .Build();

app.RegisterMiddleware();
app.RegisterSubscriberApi();
app.RegisterChannelApi();
app.RegisterPublishApi();

LogWriter.Instance.LogAsync(new Channel(), LogType.Trace, "System is ready.");
app.Run();