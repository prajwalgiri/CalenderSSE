using Microsoft.Net.Http.Headers;
using System.Reflection.PortableExecutable;

namespace SSECounterApi
{
    public class CalenderService : ICalenderService
    {
        private List<Event> _events;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        public CalenderService(IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _events = new List<Event>();
            _userService = userService;
        }
        //start connection with client 
        public async Task ConnectAsync(CancellationToken cancellationToken, string name)
        {
            if (_userService.AddUser(name))
            {
                _httpContextAccessor.HttpContext.Response.Headers.Append(HeaderNames.ContentType, "text/event-stream");
                while (!cancellationToken.IsCancellationRequested)
                {
                    AddDummyEvents();
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
            _events.Add(@event);
          
        }
        private async Task WriteEvents(CancellationToken cancellationToken)
        {
            foreach (var @event in _events)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _httpContextAccessor.HttpContext.Response.WriteAsync($"data: {@event.Name} {@event.EventDate}\n\n", cancellationToken);
                await _httpContextAccessor.HttpContext.Response.Body.FlushAsync(cancellationToken);
            }
            //Event @event = new Event("",DateTime.Now );
            //cancellationToken.ThrowIfCancellationRequested();
            //await _httpContextAccessor.HttpContext.Response.WriteAsync($"data: {@event.Name} {@event.EventDate}\n\n", cancellationToken);
            //await _httpContextAccessor.HttpContext.Response.Body.FlushAsync(cancellationToken);
            await Task.Delay(1000);
        }
        private void AddDummyEvents()
        {
            _events.Add(new Event("Event 2 Description", DateTime.Now));
            _events.Add(new Event("Event 3 Description", DateTime.Now));
            _events.Add(new Event("Event 4 Description", DateTime.Now));
        }

    }
    public interface ICalenderService
    {
        Task ConnectAsync(CancellationToken cancellationToken, string name);
        Task AddEvent(Event @event, CancellationToken cancellationToken);
    }
    public record Event(string Name, DateTime EventDate);

}
