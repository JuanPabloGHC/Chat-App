using Chat_App.Functions;
using Chat_App.Data.Entities;
using Chat_App.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chat_App.Data.Repository
{
    public class UserRepository: IUserRepository
    {
        #region < DATA MEMBERS >

        private static UserRepository instance;

        #endregion

        #region < CONSTRUCTORS >

        private UserRepository() { }

        #endregion

        #region < PATTERN IMPLEMENTATIONS >

        #region < SINGLETON >

        public static UserRepository GetInstance()
        {
            if (instance == null)
                instance = new UserRepository();

            return instance;
        }

        #endregion

        #endregion

        #region < PUBLIC METHODS >

        public async Task<List<User>> GetAll(string account, string filter = "")
        {
            using (var db = new Context())
            {
                if (filter == "")
                {
                    return await db.Users
                        .Where(u => u.Account != account)
                        .ToListAsync();
                }
                // Using the filter to search
                else
                {
                    return await db.Users
                        .Where(u => u.Account != account)
                        .Where(u => u.Name.ToLower().Contains(filter) || u.LastName.ToLower().Contains(filter))
                        .ToListAsync();
                }
            }
        }

        public async Task<User?> GetUser(string account)
        {
            using (var db = new Context())
            {
                return await db.Users
                    .Where(u => u.Account == account)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<User?> GetUser(int userID)
        {
            using (var db = new Context())
            {
                return await db.Users
                    .Where(u => u.Id == userID)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<User> Create(string name, string lastName, string account, string password, string color, byte[]? photo = null)
        {
            using (var db = new Context())
            {
                // Encrypt the password
                string passCryp = SecretHasher.Hash(password);

                User user = new User(name, lastName, account, passCryp, photo, color);

                await db.Users.AddAsync(user);

                await db.SaveChangesAsync();

                return user;
            }
        }

        public async Task ModifyStatus(int userID, bool status)
        {
            using (var db = new Context())
            {
                var u = await db.Users
                    .Where(u => u.Id == userID)
                    .FirstAsync();

                u.Status = false;

                db.SaveChanges();
            }
        }

        public async Task ModifyPhoto(int userID, byte[]? photo)
        {
            User? user = await this.GetUser(userID);

            using (var db = new Context())
            {
                user.Photo = photo;

                await db.SaveChangesAsync();
            }
        }

        public async Task<Boolean> Login(string account, string password)
        {
            User? user = await this.GetUser(account);

            using (var db = new Context())
            {
                // Verify encrypted password
                if (SecretHasher.Verify(password, user?.Password))
                {
                    user.Status = true;
                    
                    await db.SaveChangesAsync();

                    return true;
                }

                return false;
            }
        }

        #endregion

    }
}