using Model.DTO;
using Model.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public int UserRegistration(UserRegistrationDTO userRegistrationDTO)
        {
            return userRepository.UserRegistration(userRegistrationDTO);
        }

        public UserDetails UserLogin(UserLoginDTO userLoginDTO)
        {
            return userRepository.UserLogin(userLoginDTO);
        }
    }
}
