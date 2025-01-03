using System.Collections.Concurrent;
using System.Text.Json;

namespace SSEApi.CalenderService
{
    public class CalenderAdapter(CalenderService service)
    {

        private readonly ConcurrentQueue<Event> _buffer = new();
        private CancellationTokenSource _cts;
        public Task SendEvent(Event @event)
        {
            _buffer.Enqueue(@event);
            return Task.CompletedTask;


        }
        
        public async Task ConnectCalander(IHttpContextAccessor context,CancellationToken cancellationToken,string name)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var error = await service.TryAddUser(this, name);
            if (error != null)
            {
                context.HttpContext.Response.StatusCode = 400;
                await context.HttpContext.Response.WriteAsync(error, cancellationToken);
                return;
            }
            context.HttpContext.Response.Headers.Add("Content-Type", "text/event-stream");
            while (!_cts.IsCancellationRequested)
            {
                if (_buffer.TryDequeue(out var @event))
                {
                    await context.HttpContext.Response.WriteAsync($"data: ", cancellationToken);
                    await JsonSerializer.SerializeAsync(context.HttpContext.Response.Body, @event, cancellationToken: cancellationToken);
                    await context.HttpContext.Response.WriteAsync($"\n\n", cancellationToken);
                    await context.HttpContext.Response.Body.FlushAsync(cancellationToken);
                }
            }
        }
        public Task CloseConnection(string reason)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }
    }
}
