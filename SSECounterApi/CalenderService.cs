using Microsoft.Net.Http.Headers;
using System.Reflection.PortableExecutable;

namespace SSECounterApi
{
    public class CalenderService : ICalenderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly ICalenderManager _calenderManager;
        public CalenderService(IHttpContextAccessor httpContextAccessor, IUserService userService, ICalenderManager calenderManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _calenderManager = calenderManager;
        }
        //start connection with client 
        public async Task ConnectAsync(CancellationToken cancellationToken, string name)
        {
            if (_userService.AddUser(name))
            {
                _httpContextAccessor.HttpContext.Response.Headers.Append(HeaderNames.ContentType, "text/event-stream");
                while (!cancellationToken.IsCancellationRequested)
                {
                    await WriteEvents(cancellationToken);
                }
            }
            else
            {
                await _httpContextAccessor.HttpContext.Response.WriteAsync($"Login Failed for User: {name}", cancellationToken);
                await _httpContextAccessor.HttpContext.Response.Body.FlushAsync(cancellationToken);
            }

        }
        //disconnect the client 
        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            _httpContextAccessor.HttpContext.Response.CompleteAsync();
        }
        public async Task AddEvent(Event @event, CancellationToken cancellationToken)
        {
          await  _calenderManager.Add(@event, cancellationToken);

        }
        private async Task WriteEvents(CancellationToken cancellationToken)
        {
            foreach (var @event in _calenderManager.GetAllUnsent(cancellationToken))
            {
                await _calenderManager.MarkAsSent(@event, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                await _httpContextAccessor.HttpContext.Response.WriteAsync($"data: {@event.Name} {@event.EventDate}\n\n", cancellationToken);
                await _httpContextAccessor.HttpContext.Response.Body.FlushAsync(cancellationToken);
            }
            //Event @event = new Event("",DateTime.Now );
            //cancellationToken.ThrowIfCancellationRequested();
            //await _httpContextAccessor.HttpContext.Response.WriteAsync($"data: {@event.Name} {@event.EventDate}\n\n", cancellationToken);
            //await _httpContextAccessor.HttpContext.Response.Body.FlushAsync(cancellationToken);
            await Task.Delay(1000); // simulate delay because the client kept freezing
        }
        

    }
    public interface ICalenderService
    {
        Task ConnectAsync(CancellationToken cancellationToken, string name);
        Task AddEvent(Event @event, CancellationToken cancellationToken);
    }
    public record Event(string Name, DateTime EventDate);

}
