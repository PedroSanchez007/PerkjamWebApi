using Perkjam.API.Entities;
using System.Linq;

namespace Perkjam.API.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly PerkContext _context;

        public UserRepository(PerkContext context)
        {
            _context = context;
        }

        public User[] GetAllUsers()
        {
            IQueryable<User> query = _context.Users;

            // Order It
            query = query.OrderByDescending(c => c.Email);

            return query.ToArray();
        }

        public User GetUser(int id)
        {
            IQueryable<User> query = _context.Users;

            // Query It
            query = query.Where(c => c.Id == id);

            return query.FirstOrDefault();
        }

        public User GetUser(string email)
        {
            IQueryable<User> query = _context.Users;

            // Query It
            query = query.Where(c => c.Email == email);

            return query.FirstOrDefault();
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
        }

        public bool Save()
        {
            // Only return success if at least one row was changed
            return (_context.SaveChanges()) > 0;
        }
    }
}