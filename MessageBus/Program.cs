using MessageBus;
using CommonMessage.FormRequest;
using MessageBus.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dockerConnectionString = builder.Configuration.GetConnectionString("Docker");
var migrationConnectionString = builder.Configuration.GetConnectionString("Migration");
builder.Services.AddDbContext<DataContext>(options =>
options.UseMySql(dockerConnectionString, ServerVersion.AutoDetect(dockerConnectionString))
//options.UseMySql(migrationConnectionString, ServerVersion.AutoDetect(migrationConnectionString))
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/channels", async (DataContext db) =>
{
    var channels = await db.Channels
        .Include(c => c.Subscribers)
        .Include(c => c.Messages)
        .ToListAsync();
    return Results.Ok(channels);
});

app.MapGet("/subscribers", async (DataContext db) =>
{
    var subscribers = await db.Subscribers
        .Include(s => s.Channels)
        .Include(s => s.Messages)
        .ToListAsync();
    return Results.Ok(subscribers);
});

app.MapPost("/publish", async (DataContext db, PostPublish request) =>
{
    var channel = await db.Channels.FindAsync(request.ChannelId);
    if (channel == null)
    {
        return Results.BadRequest("channel is not found");
    }

    var message = new Message();
    message.PublisherName = request.Name;
    message.Channel = channel;

    db.Messages.Add(message);

    var subscribers = channel.Subscribers;

    foreach (var subscriber in subscribers)
    {
        var messageSubscriber = new MessageSubscriber();
        messageSubscriber.Subscriber = subscriber;
        messageSubscriber.Message = message;
        db.MessageSubscribers.Add(messageSubscriber);
    }
    await db.SaveChangesAsync();

    return Results.Ok();
});

//app.MapPost("/subscribe", async (DataContext db, PostSubscribe request) =>
//{
//    var channel = await db.Channels.FindAsync(request.ChannelId);
//    if (channel == null)
//    {
//        return Results.BadRequest();
//    }

//    var subscriber = await db.Subscribers.FindAsync(request.SubscriberId);
//    if (subscriber == null)
//    {
//        return Results.BadRequest();
//    }

//    db.MessageSubscribers.Add
//});

app.Run();
