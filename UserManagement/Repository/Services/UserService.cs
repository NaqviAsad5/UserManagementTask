using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Models.DbModels;
using UserManagement.Models.DTOs;
using UserManagement.Repository.Interfaces;

namespace UserManagement.Repository.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICommonService _commonService;

        public UserService(ApplicationDbContext dbContext,ICommonService commonService)
        {
            _dbContext = dbContext;
            _commonService = commonService;
        }

        public async Task<UserReadDto> AddEditUserAsync(UserWriteDto user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User data is required.");

            tblUser? existingUser = null;

            if (user.Id > 0) // update case
                existingUser = await _dbContext.tblUser.FindAsync(user.Id);

            if (existingUser == null) // create case
            {
                // email check for new user
                bool emailExists = await _dbContext.tblUser.AnyAsync(u => u.Email == user.Email);
                if (emailExists)
                    throw new InvalidOperationException("A user with this email already exists.");

                var newUser = new tblUser
                {
                    Name = user.Name,
                    Email = user.Email,
                    Password = await _commonService.HashPasswordAsync(user.Password!)
                };

                await _dbContext.tblUser.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();

                return new UserReadDto
                {
                    Id = newUser.Id,
                    Name = newUser.Name,
                    Email = newUser.Email
                };
            }
            else // update case
            {
                // email check 
                bool emailExists = await _dbContext.tblUser
                    .AnyAsync(u => u.Email == user.Email && u.Id != user.Id);
                if (emailExists)
                    throw new InvalidOperationException("Another user with this email already exists.");

                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                // password should be updated in forgot password,but forgot password not mentioned in task

                if (!string.IsNullOrWhiteSpace(user.Password))
                    existingUser.Password = await _commonService.HashPasswordAsync(user.Password);

                _dbContext.tblUser.Update(existingUser);
                await _dbContext.SaveChangesAsync();

                return new UserReadDto
                {
                    Id = existingUser.Id,
                    Name = existingUser.Name,
                    Email = existingUser.Email
                };
            }
        }



        public async Task<UserReadDto?> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _dbContext.tblUser.FindAsync(id);

                if (user == null)
                    return null;

                // mapping
                return new UserReadDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                   // due to security we don't return passwords
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching user by ID.", ex);
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _dbContext.tblUser.FindAsync(id);

                if (user == null)
                    return false; 

                _dbContext.tblUser.Remove(user);
                var result = await _dbContext.SaveChangesAsync();

                return result > 0;
            }
            catch (DbUpdateException ex)
            {
                // db related errors
                throw new Exception("Database error occurred while deleting user.", ex);
            }
            catch (Exception ex)
            {
                // unexpected errors
                throw new Exception("An unexpected error occurred while deleting user.", ex);
            }
        }

        public async Task<PagedResult<UserReadDto>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            try
            {
                var totalCount = await _dbContext.tblUser.CountAsync();

                var users = await _dbContext.tblUser
                    .OrderBy(u => u.Id) 
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserReadDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email
                    })
                    .ToListAsync();

                return new PagedResult<UserReadDto>
                {
                    Items = users,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching users.", ex);
            }
        }


    }

}
