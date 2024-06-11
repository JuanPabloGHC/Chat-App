using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.Data.Entities
{
    [Table("Chats")]
    public class Chat
    {
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
    }
}
