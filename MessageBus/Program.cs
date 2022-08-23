global using MessageBus;
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
    return await db.Channels.Include(o => o.Subscribers).ToListAsync();
});

app.Run();
