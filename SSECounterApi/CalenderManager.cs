using System.Runtime.CompilerServices;

namespace SSECounterApi
{
    public class CalenderManager : ICalenderManager
    {
        private readonly List<Event> _events;
        public async Task Add(Event @event, CancellationToken cancellationToken)
        {
            await Task.Run(()=> _events.Add(@event));
        }

        public List<Event> GetAll(CancellationToken cancellationToken)
        {
           return _events.ToList();
        }
    }
    public interface ICalenderManager
    {
        public Task Add(Event @event, CancellationToken cancellationToken);
        public List<Event> GetAll(CancellationToken cancellationToken);
    }
}
