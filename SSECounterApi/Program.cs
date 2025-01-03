using Microsoft.Net.Http.Headers;
using SSECounterApi;

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
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/sse", async Task (HttpContext ctx, ICounterService service, CancellationToken token) =>
{
    ctx.Response.Headers.Append(HeaderNames.ContentType, "text/event-stream");

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

app.MapGet("/calender", async Task (HttpContext ctx, ICalenderService service, CancellationToken token) =>
{
    var name = ctx.Request.Query["name"];
    await service.ConnectAsync(token, name);
});
app.MapGet("/calender/add", async Task (HttpContext ctx, ICalenderService service, CancellationToken token) =>
{
    var name = ctx.Request.Query["name"];
    var date = ctx.Request.Query["Date"];
    var @event = new Event(name, DateTime.Parse(date));
    await service.AddEvent(@event, token);
});
app.UseCors(myCors);
app.Run();
//await new SSEClient().GetAll();
