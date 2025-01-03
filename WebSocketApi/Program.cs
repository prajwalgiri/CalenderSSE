using WebSocketApi;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();


        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<ChatService>();
        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        //app.UseAuthorization();

        //app.MapControllers();
        app.UseWebSockets();

        app.MapGet("/", async (HttpContext context,
            ChatService chatService
            ) =>
        {
            string name = context.Request.Query["name"];
            string? touser = context.Request.Query["to"];

            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await chatService.HandleWebSocketConnection(webSocket, name, touser);
            }
            else
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Expected a WebSocket request");
            }
        });

        app.Run();
    }
}