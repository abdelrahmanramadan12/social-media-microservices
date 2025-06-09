using Domain.Entities;
using Application.Abstractions;

namespace Application.Implementations
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
            var user = await _unitOfWork.Users.FindAsync(id);
            if (user == null)
            {
                await _unitOfWork.Users.AddAsync(new User { Id = id });
            }
        }

        public async Task DeleteUser(string id)
        {
            var user = await _unitOfWork.Users.FindAsync(id);
            if (user != null)
            {
                await _unitOfWork.Users.DeleteAsync(id);
            }
        }
    }
}