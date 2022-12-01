using UmniahAssignment.Models;

namespace UmniahAssignment.Repository.Interface
{
    public interface IUserRepository
    {
        public Task<Users?> GetUser(Users user);
        public Task AddUser(Users user);
        public Task<IEnumerable<Users>> AllUsers();
        public Task UpdateUser(Users user, int? id);
        public Task<Users> GetUserById(int id);
    }
}
