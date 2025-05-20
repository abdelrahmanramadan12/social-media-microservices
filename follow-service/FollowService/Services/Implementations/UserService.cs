using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddUser(string id)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == id);
            if (user == null)
            {
                await _unitOfWork.Users.AddAsync(new User { Id = id });
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task DeleteUser(string id)
        {
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == id);
            if (user != null)
            {
                _unitOfWork.Users.Delete(user);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
