using JwtAPi.Models;

namespace JwtAPi.Repositories
{
    public class UserRepository
    {
        public static User GetUser(string username, string password)
        {

            var users = new List<User>();
            users.Add(new User
            {
                Id = 1,
                UserName = "Batman",
                Email = "batman@gmail.com",
                Password = "123456",
                Role = "Hero",
                Gender = "male"
            });
            users.Add(new User
            {
                Id = 2,
                UserName = "Robin",
                Email = "robin@gmail.com",
                Password = "654321",
                Role = "Hero",
                Gender = "male"
            });
            users.Add(new User
            {
                Id = 3,
                UserName = "Joker",
                Email = "jober@gmail.com",
                Password = "415263",
                Role = "Vilan",
                Gender = "male"
            });

            return users.Where(x => x.UserName.ToLower() == username.ToLower()
            && x.Password == password).FirstOrDefault();
        }
    }
}
