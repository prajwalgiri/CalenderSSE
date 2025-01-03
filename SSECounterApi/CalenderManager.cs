using System.Runtime.CompilerServices;

namespace SSECounterApi
{
    public class CalenderManager : ICalenderManager
    {
        private readonly List<Event> _events;
        public CalenderManager()
        {
            _events = new List<Event>();
            AddDummyEvents();
        }
        public async Task Add(Event @event, CancellationToken cancellationToken)
        {
            await Task.Run(()=> _events.Add(@event));
        }

        public List<Event> GetAll(CancellationToken cancellationToken)
        {
           return _events.ToList();
        }
        private void AddDummyEvents()
        {
            _events.Add(new Event("Event 2 Description", DateTime.Now));
            _events.Add(new Event("Event 3 Description", DateTime.Now));
            _events.Add(new Event("Event 4 Description", DateTime.Now));
        }
    }
    public interface ICalenderManager
    {
        public Task Add(Event @event, CancellationToken cancellationToken);
        public List<Event> GetAll(CancellationToken cancellationToken);
    }
}
