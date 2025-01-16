using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat_App.Data.Entities
{
    [Table("Chats")]
    public class Chat
    {
        #region < PROPERTIES >

        [Key]
        public int Id { get; set; }

        public int User1Id { get; set; }
        [ForeignKey("User1Id")]
        public User? User1 { get; set; }

        public int User2Id { get; set; }
        [ForeignKey("User2Id")]
        public User? User2 { get; set; }
        public bool Seen1 { get; set; }

        public bool Seen2 { get; set; }

        public virtual ICollection<Message>? Messages { get; set; }

        #endregion

        #region < CONSTRUCTORS >

        public Chat() { }

        public Chat(int user1ID, int user2ID, bool seen1, bool seen2)
        {
            this.User1Id = user1ID;
            this.User2Id = user2ID;
            this.Seen1 = seen1;
            this.Seen2 = seen2;
        }

        #endregion

    }
}