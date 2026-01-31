using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using YapperzAPI.Dtos.Chatroom;
using YapperzAPI.Dtos.Users;
using YapperzAPI.Models;
using YapperzAPI.Services;
using YapperzAPI.Services.Interfaces;

namespace YapperzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;

        public UsersController(IUsersService userService)
        {
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserSignUpDto userSignUpDto)
        {
            try
            {
                UsersDto user = await _userService.SignUpAsync(userSignUpDto);
                return Created("Created User " + user.Username + " successfully!", user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            var user = await _userService.AuthenticateUser(request);
            if (user is null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsersDto>> GetProfile(int id)
        {
            var user = await _userService.GetProfileAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("{id:int}/room")]
        public async Task<ActionResult<ChatroomDto>> GetUsersChatroom(int id)
        {
            try
            {
                var chatroomDto = await _userService.GetUsersChatroomAsync(id);
                if (chatroomDto is null)
                {
                    return NotFound();
                }

                return Ok(chatroomDto);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
