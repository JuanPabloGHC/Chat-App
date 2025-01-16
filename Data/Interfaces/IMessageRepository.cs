using Chat_App.Data.Entities;

namespace Chat_App.Data.Interfaces
{
    public interface IMessageRepository
    {
        public Task<List<Message>> GetAll(int chatID);

        public Task Create(int userID, int chatID, string text, DateTime date, bool status);

        public Task ModifyStatus(int chatID, int userID);

        public Task Delete(int chatID);

    }
}