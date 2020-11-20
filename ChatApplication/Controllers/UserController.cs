using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Service.Interface;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatApplication.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }


        // POST api/<UserController>
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Post([FromBody] UserRegistrationDTO userRegistrationDTO)
        {
            try
            {
                int result = await Task.FromResult(userService.UserRegistration(userRegistrationDTO));
                if (result != -1)
                {
                    return Ok(new { success = true, Message = "Registration Successfull !! Please Check Your Registered Email For Email Verification" });
                }
                return BadRequest(new { success = false, Message = "User With Same Email Id Already Exists" });
            }
            catch (Exception e)
            {
                return BadRequest(new { success = false, Message = e.Message });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDTO)
        {
            try
            {
                var data = await Task.FromResult(userService.UserLogin(userLoginDTO));
                if (data != null)
                {
                    return Ok(new { success = true, Message = "Login Successfull", data });
                }
                return BadRequest(new { success = false, Message = "Login Failed" });
            }
            catch (Exception e)
            {
                return BadRequest(new { success = false, Message = e.Message });
            }
        }
    }
}
