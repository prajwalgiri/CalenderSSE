using System.Runtime.CompilerServices;

namespace SSECounterApi
{
    public class CalenderManager : ICalenderManager
    {
        private readonly List<Dictionary<Event, bool>> _events;
        public CalenderManager()
        {
            _events = new List<Dictionary<Event, bool>>();
            AddDummyEvents();
        }
        public async Task Add(Event @event, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var val = new Dictionary<Event, bool>();
                val.Add(@event, false);
                _events.Add(val);
            });
        }

        public List<Event> GetAll(CancellationToken cancellationToken)
        {
            return _events.SelectMany(x => x.Keys).ToList();
        }
        public List<Event> GetAllUnsent(CancellationToken cancellationToken)
        {
            return _events.Where(x => x.ContainsValue(false)).SelectMany(x => x.Keys).ToList();

        }
        public async Task MarkAsSent(Event @event, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                if (_events.Any(x => x.ContainsKey(@event)))
                {
                    _events.First(x => x.ContainsKey(@event))[@event] = true;
                }
            });

        }
        private void AddDummyEvents()
        {
            var _defaultevents = new Dictionary<Event, bool>();
            _defaultevents.Add(new Event("Event 1 Description", DateTime.Now), false);
            _defaultevents.Add(new Event("Event 2 Description", DateTime.Now), false);
            _defaultevents.Add(new Event("Event 3 Description", DateTime.Now), false);
            _events.Add(_defaultevents);
        }
    }
    public interface ICalenderManager
    {
        public Task Add(Event @event, CancellationToken cancellationToken);
        public List<Event> GetAll(CancellationToken cancellationToken);
        public List<Event> GetAllUnsent(CancellationToken cancellationToken);
        public  Task MarkAsSent(Event @event, CancellationToken cancellationToken);

    }
}
