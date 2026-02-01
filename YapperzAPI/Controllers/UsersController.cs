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

        [HttpPut("update-profile/{id}")]
        public async Task<ActionResult<UsersDto>> UpdateProfile(int id, [FromBody] UsersDto userDto)
        {
            var updatedUser = await _userService.UpdateUserProfileAsync(id, userDto);
            if (updatedUser == null)
            {
                return NotFound();
            }
            return Ok(updatedUser);
        }

        // --- FIXED ENDPOINT ---
        [HttpPut("UpdateAvatar")]
        public async Task<ActionResult<UsersDto>> UpdateAvatarFromJs([FromBody] UpdateAvatarRequest request)
        {
            // 1. Fetch the EXISTING user first.
            // This ensures we have a valid DTO with Username, Email, etc. already filled in.
            var existingUser = await _userService.GetProfileAsync(request.UserId);

            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            // 2. Modify ONLY the AvatarPath
            existingUser.AvatarPath = request.AvatarPath;

            // 3. Save the changes
            var updatedUser = await _userService.UpdateUserAvatarAsync(request.UserId, existingUser);

            if (updatedUser == null)
            {
                return NotFound("User not found during update");
            }

            return Ok(updatedUser);
        }
    }

    // Helper class for the JSON from avatar.js
    // Added 'required' to fix the CS8618 warning
    public class UpdateAvatarRequest
    {
        public int UserId { get; set; }
        public required string AvatarPath { get; set; }
    }
}