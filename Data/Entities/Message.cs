using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat_App.Data.Entities
{
    [Table("Messages")]
    public class Message
    {
        #region < PROPERTIES >

        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int ChatId { get; set; }
        [ForeignKey("ChatId")]
        public Chat? Chat { get; set; }

        public string? Text { get; set; }

        public DateTime? Date { get; set; }

        public bool Status { get; set; }

        #endregion

        #region < CONSTRUCTORS >

        public Message() { }

        public Message(int userID, int chatID, string? text, DateTime? date, bool status)
        {
            this.UserId = userID;
            this.ChatId = chatID;
            this.Text = text;
            this.Date = date;
            this.Status = status;
        }

        #endregion

    }
}