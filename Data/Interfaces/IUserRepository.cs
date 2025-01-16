using Chat_App.Data.Entities;

namespace Chat_App.Data.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAll(string account, string filter = "");
        
        public Task<User?> GetUser(string account);
        
        public Task<User?> GetUser(int userID);
        
        public Task<User> Create(string name, string lastName, string account, string password, string color, byte[]? photo = null);

        public Task ModifyStatus(int userID, bool status);

        public Task ModifyPhoto(int userID, byte[]? photo);

        public Task<Boolean> Login(string account, string password);

    }
}