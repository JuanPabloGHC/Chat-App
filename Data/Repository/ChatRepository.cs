using Chat_App.Data.Entities;
using Chat_App.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chat_App.Data.Repository
{
    public class ChatRepository: IChatRepository
    {
        #region < DATA MEMBERS >

        private static ChatRepository instance;

        #endregion

        #region < CONSTRUCTORS >

        private ChatRepository() { }

        #endregion

        #region < PATTERN IMPLEMENTATIONS >

        #region < SINGLETON >

        public static ChatRepository GetInstance()
        {
            if (instance == null)
                instance = new ChatRepository();

            return instance;
        }

        #endregion

        #endregion

        #region < PUBLIC METHODS >

        public async Task<Chat?> GetChat(int user1ID, int user2ID)
        {
            using (var db = new Context())
            {
                return await db.Chats
                    .Include(c => c.Messages)
                    .Where(c => (c.User1Id == user1ID && c.User2Id == user2ID) || (c.User1Id == user2ID && c.User2Id == user1ID))
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<Chat?> GetChat(int chatID)
        {
            using (var db = new Context())
            {
                return await db.Chats
                    .Include(c => c.Messages)
                    .Where(c => c.Id == chatID)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<Message?> GetLastMessage(int user1ID, int user2ID)
        {
            Chat? chat = await this.GetChat(user1ID, user2ID);

            using (var db = new Context())
            {
                return chat?.Messages?
                    .OrderByDescending(m => m.Date)
                    .FirstOrDefault();
            }
        }

        public async Task<Chat> Create(int user1ID, int user2ID)
        {
            using (var db = new Context())
            {
                Chat _chat = new Chat(user1ID, user2ID, true, false);
                
                await db.Chats.AddAsync(_chat);

                await db.SaveChangesAsync();

                return _chat;
            }
        }

        public async Task ModifyStatus(int chatID, int user1ID)
        {
            Chat? chat = await this.GetChat(chatID);

            using (var db = new Context())
            {
                if (chat?.User1Id == user1ID)
                {
                    chat.Seen1 = true;
                    chat.Seen2 = false;
                }
                else
                {
                    chat.Seen2 = true;
                    chat.Seen1 = false;
                }

                await db.SaveChangesAsync();
            }
        }

        public async Task SeenChat(int chatID, int userID)
        {
            Chat? chat = await this.GetChat(chatID);

            using (var db = new Context())
            {
                if (chat.User1Id == userID)
                {
                    chat.Seen1 = true;
                }
                else
                {
                    chat.Seen2 = true;
                }

                await db.SaveChangesAsync();
            }
        }

        public async Task Delete(int user1ID, int user2ID)
        {
            Chat? chat = await this.GetChat(user1ID, user2ID);

            MessageRepository _messageRepository = MessageRepository.GetInstance();

            await _messageRepository.Delete(chat.Id);

        }

        #endregion

    }
}