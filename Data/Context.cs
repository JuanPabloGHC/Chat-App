using Chat_App.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chat_App.Data
{
    public class Context : DbContext
    {
        #region < DB SETS DEFINITION >

        public DbSet<User> Users { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<Message> Messages { get; set; }

        #endregion

        #region < EVENT HANDLING >

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=P02045832\\SQLEXPRESS;Database=ChatAppDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
        }

        #endregion

    }
}