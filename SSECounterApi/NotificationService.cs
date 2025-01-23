using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using System.Xml.Linq;

namespace SSECounterApi
{
    public class NotificationService : INotificationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly INotificationManager _notificationManager;
        public NotificationService(IHttpContextAccessor httpContextAccessor, IUserService userService, INotificationManager notificationManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _notificationManager = notificationManager;
        }

        public async Task AddNotification(Notification @Notification, List<string> users, CancellationToken cancellationToken)
        {
            await _notificationManager.Add(@Notification, users, cancellationToken);
        }

        public async Task ConnectAsync(CancellationToken cancellationToken, string name)
        {
            if (_userService.AddUser(name))
            {
                _httpContextAccessor.HttpContext.Response.Headers.Append(HeaderNames.ContentType, "text/event-stream");
                while (!cancellationToken.IsCancellationRequested)
                {
                    await WriteNotificationToStream(name, cancellationToken);
                }

            }
            else
            {
                await _httpContextAccessor.HttpContext.Response.WriteAsync($"Login Failed for User: {name}", cancellationToken);
                await _httpContextAccessor.HttpContext.Response.Body.FlushAsync(cancellationToken);
            }
        }
        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            _httpContextAccessor.HttpContext.Response.CompleteAsync();
        }

        public async Task MarkAsSent(string id,string name, CancellationToken cancellationToken)
        {
            Guid guid = Guid.Empty;
            Guid.TryParse(id, out guid);
            if (id == null || guid == Guid.Empty)
            {
                await _httpContextAccessor.HttpContext.Response.WriteAsync($"Notification Id is Invalid.", cancellationToken);
                await _httpContextAccessor.HttpContext.Response.Body.FlushAsync(cancellationToken);

            }
            await _notificationManager.MarkAsSent(guid,name, cancellationToken);
        }

        private async Task WriteNotificationToStream(string name, CancellationToken cancellationToken)
        {
            var allNotifications = await _notificationManager.GetAllUnsent(name, cancellationToken);
            foreach (var notification in allNotifications)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var payload = JsonSerializer.Serialize(notification);
                await _httpContextAccessor.HttpContext.Response.WriteAsync($"data: {payload} \n\n", cancellationToken);
                await _httpContextAccessor.HttpContext.Response.Body.FlushAsync(cancellationToken);
                await _notificationManager.MarkAsSent(notification.id,name, cancellationToken);

            }
            await Task.Delay(1000);// simulate delay because the client kept freezing
        }
    }
    public interface INotificationService
    {
        Task ConnectAsync(CancellationToken cancellationToken, string name);
        Task AddNotification(Notification @Notification, List<string> users, CancellationToken cancellationToken);
        Task MarkAsSent(string id,string name, CancellationToken cancellationToken);
    }
}
