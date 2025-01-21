using Microsoft.Extensions.Logging;

namespace SSECounterApi
{
    public class NotificationManager : INotificationManager
    {
        private readonly List<Dictionary<Notification, string[] >> _notifications;
        public NotificationManager()
        {
            _notifications = new List<Dictionary<Notification, string[]>>();
        }
        public async Task Add(Notification @Notification,string[] users, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var val = new Dictionary<Notification, string[]>();
                val.Add(@Notification, users);
                _notifications.Add(val);
            });
        }

        public  List<Notification> GetAll(string userid, CancellationToken cancellationToken)
        {
            // return  _notifications.SelectMany(x=> x.Keys).Where(user=> user.;
            return null;
        }
    }
    public interface INotificationManager
    {
        public Task Add(Notification @Notification,string[] users, CancellationToken cancellationToken);
        public List<Notification> GetAll(string userid,CancellationToken cancellationToken);

    }
    public record Notification(string msg);
}
