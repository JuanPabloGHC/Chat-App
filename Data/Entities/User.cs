using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat_App.Data.Entities
{
    [Table("Users")]
    public class User
    {
        #region < PROPERTIES >

        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? LastName { get; set; }

        public string? Account {  get; set; }

        public string? Password { get; set; }

        public bool Status { get; set; }

        public byte[]? Photo { get; set; }

        public string? Color { get; set; }

        #endregion

        #region < CONSTRUCTORS >

        public User() { }

        public User(string? name, string? lastName, string? account, string? password, byte[]? photo, string? color)
        {
            this.Name = name;
            this.LastName = lastName;
            this.Account = account;
            this.Password = password;
            this.Photo = photo;
            this.Color = color;
        }

        #endregion

    }
}