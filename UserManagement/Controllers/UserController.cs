using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using UserManagement.Models.DTOs;
using UserManagement.Repository.Interfaces;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserReadDto>>> CreateUser([FromBody] UserWriteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid request data.",
                    Data = null
                });

            try
            {
                var createdUser = await _userService.AddEditUserAsync(dto);

                return CreatedAtAction(nameof(GetUserById), new { id = createdUser },
                    new ApiResponse<UserReadDto>
                    {
                        Success = true,
                        Message = "User created successfully.",
                        Data = createdUser
                    });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation failed while creating user.");
                return Conflict(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user.");
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "An unexpected error occurred.",
                    Data = null
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<UserReadDto>>>> GetAllUsers(
          [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _userService.GetAllUsersAsync(pageNumber, pageSize);

                return Ok(new ApiResponse<PagedResult<UserReadDto>>
                {
                    Success = true,
                    Message = "Users fetched successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users.");
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "An unexpected error occurred."
                });
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserReadDto>>> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                return Ok(new ApiResponse<UserReadDto>
                {
                    Success = true,
                    Message = "User fetched successfully",
                    Data = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching user with Id {id}");
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "An unexpected error occurred"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UserReadDto>>> UpdateUser(int id, [FromBody] UserWriteDto userDto)
        {
            if (userDto == null || id != userDto.Id)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid user data or mismatched Id."
                });
            }
            userDto.Id = id;
            try
            {
                var updatedUserId = await _userService.AddEditUserAsync(userDto);

                // Get updated user details
                var updatedUser = await _userService.GetUserByIdAsync(updatedUserId.Id);

                return Ok(new ApiResponse<UserReadDto>
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = updatedUser
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation failed while creating user.");
                return Conflict(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with Id {id}");
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while updating the user."
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);

                if (!result)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "User not found."
                    });
                }

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "User deleted successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with Id {id}");
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while deleting the user."
                });
            }
        }


    }

}
