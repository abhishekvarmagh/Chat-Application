using Model.DTO;
using Model.Models;

namespace Service.Interface
{
    public interface IUserService
    {
        int UserRegistration(UserRegistrationDTO userRegistrationDTO);
        UserDetails UserLogin(UserLoginDTO userLoginDTO);
    }
}
