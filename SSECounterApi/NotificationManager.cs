namespace SSECounterApi
{
    public class NotificationManager : INotificationManager
    {
        private readonly List<Tuple<Notification,string,bool>> _notifications; //Notification,user,isSent 
        public NotificationManager()
        {
            _notifications = new List<Tuple<Notification, string, bool>>();
            AddDummyNotifications();
        }
        public async Task Add(Notification @Notification, List<string> users, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                if (users.Count == 0) users.Add("All"); 
                foreach (var user in users)
                {
                    _notifications.Add(new Tuple<Notification, string, bool>(@Notification, user, false));
                }
            });
        }

        public async Task<List<Notification>> GetAll(string userid, CancellationToken cancellationToken)
        {
            return await Task.Run(()=> _notifications.Where(n =>
            {
                var toUser = n.Item2;
                if (toUser == userid)
                {
                    return true;
                }
                else if (toUser == "All")
                    return true;
                else
                {
                    return false;
                }
                }).Select(x => x.Item1).ToList()
            );
        }

        public async Task MarkAsSent(Guid guid, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var notification= _notifications.Find(n => n.Item1.id == guid);
                var nkey= _notifications.Find(x=> x==notification)!;
                _notifications.Remove(nkey);
            });
        }

        private void AddDummyNotifications()
        {
            _notifications.Add(new Tuple<Notification, string, bool>(new Notification(Guid.NewGuid(), "Welcome"), "All", false));
            _notifications.Add(new Tuple<Notification, string, bool>(new Notification(Guid.NewGuid(), "This is a Test notification"), "All", false));
            _notifications.Add(new Tuple<Notification, string, bool>(new Notification(Guid.NewGuid(), "Happy to have you"), "All", false));
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
