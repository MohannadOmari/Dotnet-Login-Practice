using Dapper;
using UmniahAssignment.Context;
using UmniahAssignment.Models;
using UmniahAssignment.Repository.Interface;

namespace UmniahAssignment.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task AddUser(Users user)
        {
            // check if email exists
            var query = "SELECT 1 FROM Users WHERE Username = @Username";
            var Username = user.Username;
            // encrypt password
            string Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            using (var connection = _context.CreateConnection())
            {
                var isRegisterd = await connection.QuerySingleOrDefaultAsync(query, new { Username });
                if (isRegisterd == null)
                {
                    query = "INSERT INTO Users (Username, FullName, Password, PhoneNumber, CreatedAt, UpdatedAt, Active)"
                        + "VALUES (@Username, @FullName, @Password, @PhoneNumber, @CreatedAt, @UpdatedAt, @Active)";
                    var parameters = new DynamicParameters();
                    parameters.Add("Username", user.Username);
                    parameters.Add("FullName", user.FullName);
                    parameters.Add("Password", Password);
                    parameters.Add("PhoneNumber", user.PhoneNumber);
                    parameters.Add("CreatedAt", DateTime.Now);
                    parameters.Add("UpdatedAt", DateTime.Now);
                    parameters.Add("Active", "Yes");
                    await connection.ExecuteAsync(query, parameters);
                }
            }
        }

        public async Task<IEnumerable<Users>> AllUsers(string? searchString)
        {
            string query;
            if (searchString == null)
            {
                query = "SELECT * FROM Users";
            }
            else
            {
                query = $"SELECT * FROM Users WHERE FullName = {searchString}";
            }
            using(var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<Users>(query);
                return users.ToList();
            }

        }

        public async Task<Users?> GetUser(Users user)
        {
            var query = "SELECT * FROM Users WHERE Username = @Username AND Active = 'Yes'";
            var Username = user.Username;
            using (var connection = _context.CreateConnection())
            {
                var userLogin = await connection.QuerySingleOrDefaultAsync<Users>(query, new { Username });
                
                if (userLogin != null)
                {
                    var hashedPassword = userLogin.Password;
                    bool passwordMatch = BCrypt.Net.BCrypt.Verify(user.Password, hashedPassword);
                    if (passwordMatch)
                    {
                        return userLogin;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<Users> GetUserById(int id)
        {
            var query = "SELECT * FROM Users WHERE Id = @id";
            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<Users>(query, new { id });
                return user;
            }
        }

        public async Task UpdateUser(Users user, int? id)
        {
            var query = "UPDATE Users SET Username = @Username, FullName = @FullName, PhoneNumber = @PhoneNumber, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            using(var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("Username", user.Username);
                parameters.Add("FullName", user.FullName);
                parameters.Add("PhoneNumber", user.PhoneNumber);
                parameters.Add("UpdatedAt", DateTime.Now);
                parameters.Add("Id", id);
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
