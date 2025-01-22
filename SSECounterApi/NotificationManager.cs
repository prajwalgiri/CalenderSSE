namespace SSECounterApi
{
    public class NotificationManager : INotificationManager
    {
        private readonly List<Dictionary<Notification, List<string>>> _notifications;
        public NotificationManager()
        {
            _notifications = new List<Dictionary<Notification, List<string>>>();
            AddDummyNotifications();
        }
        public async Task Add(Notification @Notification, List<string> users, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var val = new Dictionary<Notification, List<string>>();
                val.Add(@Notification, users);
                _notifications.Add(val);
            });
        }

        public async Task<List<Notification>> GetAll(string userid, CancellationToken cancellationToken)
        {
            return await Task.Run(()=> _notifications.Where(n =>
                     {
                         var users = n.Values.Cast<List<string>>().First();
                             if (users.Any())
                             {
                                 if (users.Contains(userid))
                                 {
                                     return true;
                                 }
                                 else { return false; }
                             }
                             else
                             {
                                 return true;
                             }
                     }).SelectMany(x => x.Keys).ToList()
                     );
        }

        public async Task MarkAsSent(Guid guid, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
            var notification= _notifications.Select(n => n.Keys.First(x => x.id == guid)).First();
           var nkey= _notifications.Find(x=> x.ContainsKey(notification));
            _notifications.Remove(nkey);
            });
        }

        private void AddDummyNotifications()
        {
            var _dummy= new Dictionary<Notification, List<string>>();
            _dummy.Add(new Notification(Guid.NewGuid(),"Welcome"), new List<string>());
            _dummy.Add(new Notification(Guid.NewGuid(),"This is a Test notification"), new List<string>());
            _dummy.Add(new Notification(Guid.NewGuid(), "Happy to have you"), new List<string>());
            _notifications.Add(_dummy);
        }
    }
    public interface INotificationManager
    {
        public Task Add(Notification @Notification, List<string> users, CancellationToken cancellationToken);
        public Task<List<Notification>> GetAll(string userid, CancellationToken cancellationToken);
        public Task MarkAsSent(Guid id, CancellationToken cancellationToken);

    }
    public record Notification(Guid id,string msg);
    public record AddNotification(Notification notification,string users);
}
