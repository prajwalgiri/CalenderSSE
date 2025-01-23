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
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<INotificationManager, NotificationManager>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.MapGet("/notifications", async Task (HttpContext ctx, INotificationService service, CancellationToken token) =>
{
    var name = ctx.Request.Query["name"];
    await service.ConnectAsync(token, name);
});
app.MapGet("/notification/mark-as-read", async Task (HttpContext ctx, INotificationService service, CancellationToken token) =>
{
    var id = ctx.Request.Query["id"];
    var name = ctx.Request.Query["user"];
    await service.MarkAsSent(id,name, token);
});
app.MapPost("/notifications/add", async Task (HttpContext ctx,
    INotificationService service,
    CancellationToken token
    
    ) =>
{
    var users = ctx.Request.Form["users"];
    var msg = ctx.Request.Form["msg"];
    var userList= JsonSerializer.Deserialize<List<string>>(users)??new List<string>();
    var @notification = new Notification(Guid.NewGuid(),msg);
    await service.AddNotification(@notification,userList, token);
});

app.UseCors(myCors);
app.Run();
//await new SSEClient().GetAll();
