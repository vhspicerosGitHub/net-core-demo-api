using NetCoreDemoApi.Common;
using NetCoreDemoApi.Model;
using NetCoreDemoApi.Repositories;

namespace NetCoreDemoApi.Services
{
    public class AuthService
    {
        private readonly IUserRepository userRepository;

        public AuthService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<User> Login(string email, string password)
        {
            var user = await userRepository.GetByEmail(email);
            if (user == null)
                throw new BusinessException("User not found");

            if (user.Password != password)
                throw new BusinessException("Password incorrect");
            return user;

        }
    }
}
