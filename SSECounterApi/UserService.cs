using System.Globalization;

namespace SSECounterApi
{
    public class UserService: IUserService
    {
        public List<string> Users;
        public UserService() { 
            Users= new List<string>();
        }
        public bool AddUser(string name)
        {
            if(Users.Contains(name))
            {
                return false;
            }
            else
            {
                Users.Add(name);
            }
            return true;
        }
    }
    public interface IUserService
    {
        public bool AddUser(string name);
    }
}
