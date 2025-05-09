using Google.Apis.Auth;
using LibraryManagement.DTOs.Response;
using LibraryManagement.Models;
using LibraryManagement.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetUsersWithBorrowedBooksAsync(int id)
        {
            return await _userRepository.GetUsersWithBorrowedBooksAsync(id);
        }

        public async Task<User> AddAsync(User user)
        {
            return await _userRepository.AddAsync(user);
        }

        public async Task<bool> UpdateAsync(User user)
        {
            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<User> GetOrCreateUserAsync(GooglePayload payload)
        {
            var user = await _userRepository.GetOrCreateUserAsync(payload);

            return user;
        }
    }
}
