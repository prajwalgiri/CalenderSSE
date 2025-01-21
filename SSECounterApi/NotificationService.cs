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

        public async Task AddNotification(Event @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task ConnectAsync(CancellationToken cancellationToken, string name)
        {
            throw new NotImplementedException();
        }
    }
    public interface INotificationService
    {
        Task ConnectAsync(CancellationToken cancellationToken, string name);
        Task AddNotification(Event @event, CancellationToken cancellationToken);
    }
}
