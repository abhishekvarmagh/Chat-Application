using Model.DTO;
using Model.Models;

namespace Repository.Interface
{
    public interface IUserRepository
    {
        int UserRegistration(UserRegistrationDTO userRegistrationDTO);

        UserDetails UserLogin(UserLoginDTO userLoginDTO);
    }
}
