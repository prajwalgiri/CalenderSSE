using System.Collections.Concurrent;

namespace SSEApi.CalenderService
{
    public class CalenderService(ILogger<CalenderService> logger)
    {
        private  ConcurrentDictionary<string, CalenderAdapter> Clients = new();
        private readonly ConcurrentQueue<Event> History = new();
        public async Task AddEvent(CalenderAdapter service, Event @event)
        {
            History.Enqueue(@event);
            await BroadCastEvent(@event);
        }

        public async Task BroadCastEvent(Event @event, IEnumerable<CalenderAdapter>? receivers = null)
        {
            logger.LogInformation("Broadcasting event: {event}", @event);
            foreach (var clientCalender in Clients.Values)
            {
                await clientCalender.SendEvent(@event);
            }
        }
        public async Task<string?> TryAddUser(CalenderAdapter adapter, string name)
        {
            if (!TryAddUser(name, adapter))
            {
                return $"Name '{name}' already taken";
            }

            var userConnected = new UserConnected(name);
            var everyoneElse = Clients.Where(x => x.Key != name).Select(x => x.Value);
            Clients.TryAdd(name, adapter);
            await this.SendEvent(adapter,new Event("Welcome to the calender", new DateOnly()));

            //await SendEvent(adapter, new History(History));
            //await SendEvent(adapter, new UserList(Clients.Keys));

            return null;
        }
        private bool TryAddUser(string name, CalenderAdapter adapter)
        {
            if (Clients.ContainsKey(name))
            {
                return false;
            }

            Clients.TryAdd(name, adapter);

            return true;
        }
        public Task SendEvent(CalenderAdapter adapter, Event @event)
        {
            logger.LogInformation("Sending event: {Event}", @event);

            return adapter.SendEvent(@event);
        }   

        public async Task BroadcastEvent(Event @event, IEnumerable<CalenderAdapter>? receivers = null)
        {
            logger.LogInformation("Broadcasting Event: {Event}", @event);

            History.Enqueue(@event);

            foreach (var connection in receivers ?? Clients.Values)
            {
                await connection.SendEvent(@event);
            }
        }
    }
    public record Event(string Name, DateOnly EventDate);
    public record UserConnected(string Name);
    public record UserList(IEnumerable<string> Users):Event(nameof(UserList), new DateOnly());
    public record History(IEnumerable<Event> Events) : Event(nameof(History),new DateOnly());


}
