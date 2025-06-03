using Application.Abstractions;
using Application.DTOs;

namespace Application.Services
{
    public class ChatCommandService : IChatCommandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatCommandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task CreateConversationAsync(NewConversationDTO conversation)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMessageAsync(MessageDTO message)
        {
            throw new NotImplementedException();
        }

        public Task EditMessageAsync(MessageDTO message)
        {
            throw new NotImplementedException();
        }

        public Task SendMessageAsync(MessageDTO message)
        {
            throw new NotImplementedException();
        }
    }
}
