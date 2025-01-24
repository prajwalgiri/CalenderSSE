using System.Runtime.CompilerServices;

namespace SSECounterApi
{
    public class NotificationManager : INotificationManager
    {
                                //Notification,user,isSent ,hasread
        private readonly List<Tuple<Notification,string,bool,bool>> _notifications; 
        public NotificationManager()
        {
            _notifications = new List<Tuple<Notification, string, bool, bool>>();
        }
        public async Task Add(Notification @Notification, List<string> users, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                if (users.Count == 0) users.Add("All"); 
                foreach (var user in users)
                {
                    _notifications.Add(new Tuple<Notification, string, bool,bool>(@Notification, user, false,false));
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
        public async Task<List<Notification>> GetAllUnsent(string userid, CancellationToken cancellationToken)
        {
            return await Task.Run(() => _notifications.Where(n =>
            {
                var toUser = n.Item2;
                if (toUser == userid|| toUser == "All")
                {
                    return !n.Item3;
                }
                else
                {
                    return false;
                }
            }).Select(x => x.Item1).ToList()
            );
        }

        public async Task MarkAsSent(Guid guid,string user, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var notification = _notifications.Where(n => n.Item1.id == guid && (n.Item2 == user|| n.Item2=="All")).FirstOrDefault();
                if (notification == null) return;
                _notifications.Remove(notification);
                _notifications.Add(new Tuple<Notification, string, bool,bool>(notification.Item1,notification.Item2,true,false));
               
            });
        }
        public async Task MarkAsRead(Guid guid, string user, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var notification = _notifications.Where(n => n.Item1.id == guid && (n.Item2 == user || n.Item2 == "All")).FirstOrDefault()!;
                if (notification == null) return;
                _notifications.Remove(notification);
                var newnotification = new Notification(Guid.NewGuid(),"Read: Msg with GUID: "+notification.Item1.id);
                _notifications.Add(new Tuple<Notification, string, bool, bool>(notification.Item1, notification.Item2, notification.Item3, true));
                //_notifications.Add(new Tuple<Notification, string, bool, bool>(newnotification, user, false, false));

            });
        }

        public async Task  AddDummyNotifications(string user)
        {
            if(_notifications.FindAll(x=> x.Item2==user).Count>0) return;
           await Task.Run(() =>
            {
                _notifications.Add(new Tuple<Notification, string, bool, bool>(new Notification(Guid.NewGuid(), "Welcome"), user, false,false));
                _notifications.Add(new Tuple<Notification, string, bool, bool>(new Notification(Guid.NewGuid(), "This is a Test notification"), user, false,false));
                _notifications.Add(new Tuple<Notification, string, bool, bool>(new Notification(Guid.NewGuid(), "Happy to have you"), user, false,false));
            });       
        }
    }
    public interface INotificationManager
    {
        public Task Add(Notification @Notification, List<string> users, CancellationToken cancellationToken);
        public Task<List<Notification>> GetAll(string userid, CancellationToken cancellationToken);
        public Task<List<Notification>> GetAllUnsent(string userid, CancellationToken cancellationToken);
        public Task MarkAsSent(Guid id,string user, CancellationToken cancellationToken);
        public Task MarkAsRead(Guid id,string user, CancellationToken cancellationToken);
        public  Task AddDummyNotifications(string user);
    }
    public record Notification(Guid id,string msg);
}
