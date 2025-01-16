using Chat_App.Data.Entities;

namespace Chat_App.Data.Interfaces
{
    public interface IChatRepository
    {
        public Task<Chat?> GetChat(int user1ID, int user2ID);

        public Task<Chat?> GetChat(int chatID);

        public Task<Message?> GetLastMessage(int user1ID, int user2ID);

        public Task<Chat> Create(int user1ID, int user2ID);

        public Task ModifyStatus(int chatID, int user1ID);

        public Task SeenChat(int chatID, int userID);
        
        public Task Delete(int user1ID, int user2ID);

    }
}