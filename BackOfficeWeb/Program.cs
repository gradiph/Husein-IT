global using CommonLog;
global using CommonJson;
global using CommonMessage;
using BackOfficeWeb;
using BackOfficeWeb.Models.MessageBus;

LogWriter.Instance.LogAsync(new ChannelViewModel(), LogType.Trace, "System is starting...");

var builder = WebApplication.CreateBuilder(args);
builder.RegisterServices();

var app = builder.Build();
app.SetupMiddleware();
app.SetupRoutes();

LogWriter.Instance.LogAsync(new ChannelViewModel(), LogType.Trace, "System is ready.");

app.Run();
