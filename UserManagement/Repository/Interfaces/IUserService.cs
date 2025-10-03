using UserManagement.Models;
using UserManagement.Models.DTOs;

namespace UserManagement.Repository.Interfaces
{
    public interface IUserService
    {
        public Task<UserReadDto> AddEditUserAsync(UserWriteDto user);
        public Task<UserReadDto?> GetUserByIdAsync(int id);
        public Task<bool> DeleteUserAsync(int id);
        public Task<PagedResult<UserReadDto>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 10);

    }
}
