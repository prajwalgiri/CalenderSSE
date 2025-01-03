using SSEApi.CalenderService;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddTransient<CalenderService>();
        builder.Services.AddTransient<CalenderAdapter>();
        builder.Services.AddTransient<IHttpContextAccessor,HttpContextAccessor>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.UseCors(option=>
        {
            option.AllowAnyOrigin();
            option.AllowAnyMethod();
            option.AllowAnyHeader();
        });
        app.Run();
    }
}