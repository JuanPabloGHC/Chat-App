using Chat_App.Data.Entities;
using Chat_App.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chat_App.Data.Repository
{
    public class MessageRepository: IMessageRepository
    {
        #region < DATA MEMBERS >

        private static MessageRepository instance;

        #endregion

        #region < CONSTRUCTORS >

        private MessageRepository() { }

        #endregion

        #region < PATTERN IMPLEMENTATIONS >

        #region < SINGLETON >

        public static MessageRepository GetInstance()
        {
            if (instance == null)
                instance = new MessageRepository();

            return instance;
        }

        #endregion

        #endregion

        #region < PUBLIC METHODS >

        public async Task<List<Message>> GetAll(int chatID)
        {
            using (var db = new Context())
            {
                return await db.Messages
                        .Where(m => m.ChatId == chatID)
                        .OrderBy(m => m.Date)
                        .ToListAsync();
            }
        }

        public async Task Create(int userID, int chatID, string text, DateTime date, bool status)
        {
            using (var db = new Context())
            {
                Message _message = new Message(userID, chatID, text, date, status);

                await db.Messages.AddAsync(_message);

                await db.SaveChangesAsync();
            }
        }

        public async Task ModifyStatus(int chatID, int userID)
        {
            using (var db = new Context())
            {
                List<Message> messages = await db.Messages
                        .Where(m => m.ChatId == chatID)
                        .Where(m => m.UserId == userID && m.Status == false)
                        .ToListAsync();

                // Update status of each message
                foreach (var message in messages)
                {
                    message.Status = true;
                }

                await db.SaveChangesAsync();
            }
        }

        public async Task Delete(int chatID)
        {
            List<Message> messages = await this.GetAll(chatID);

            using (var db = new Context())
            {
                foreach (Message message in messages)
                {
                    db.Remove(message);
                }

                await db.SaveChangesAsync();
            }
        }

        #endregion

    }
}