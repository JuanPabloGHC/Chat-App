using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.Data.Entities
{
    [Table("Messages")]
    public class Message
    {
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
    }
}
