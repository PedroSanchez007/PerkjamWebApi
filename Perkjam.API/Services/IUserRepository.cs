using Perkjam.API.Entities;

namespace Perkjam.API.Services
{
  public interface IUserRepository
  {
    User[] GetAllUsers();
    User GetUser(int id);
    User GetUser(string email);
    void AddUser(User user);
    void UpdateUser(User user);
    void DeleteUser(User user);
    bool Save();
  }
}