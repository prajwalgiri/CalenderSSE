using Microsoft.Net.Http.Headers;
using SSECounterApi;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
const string myCors = "client";


builder.Services.AddCors(options =>
{
    options.AddPolicy(myCors,
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
builder.Services.AddScoped<ICounterService, CounterService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ICalenderManager, CalenderManager>();
builder.Services.AddScoped<ICalenderService, CalenderService>();
builder.Services.AddSingleton<INotificationManager, NotificationManager>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/sse", async Task (HttpContext ctx, ICounterService service, CancellationToken token) =>
{
    ctx.Response.Headers.Append(HeaderNames.ContentType, "text/event-stream");
    ctx.Response.Headers.Append(HeaderNames.CacheControl, "no-cache");
    ctx.Response.Headers.Append(HeaderNames.Connection, "keep-alive");

    var count = service.StartValue;

    while (count >= 0)
    {
        token.ThrowIfCancellationRequested();

        await service.CountdownDelay(token);

        await ctx.Response.WriteAsync($"data: {count}\n\n", cancellationToken: token);
        await ctx.Response.Body.FlushAsync(cancellationToken: token);

        count--;
    }

    await ctx.Response.CompleteAsync();
});
app.MapGet("/notifications", async Task (HttpContext ctx, INotificationService service, CancellationToken token) =>
{
    var name = ctx.Request.Query["name"];
    await service.ConnectAsync(token, name);
});
app.MapGet("/notification/mark-as-read", async Task (HttpContext ctx, INotificationService service, CancellationToken token) =>
{
    var name = ctx.Request.Query["id"];
    await service.MarkAsSent(name,token);
});
app.MapPost("/notifications/add", async Task (HttpContext ctx,
    INotificationService service,
    CancellationToken token
    
    ) =>
{
    var users = ctx.Request.Form["users"];
    var msg = ctx.Request.Form["msg"];
    var userList= JsonSerializer.Deserialize<List<string>>(msg)??new List<string>();
    var @notification = new Notification(Guid.NewGuid(),msg);
    await service.AddNotification(@notification,userList, token);
});

app.UseCors(myCors);
app.Run();
//await new SSEClient().GetAll();
