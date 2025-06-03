using Application.Abstractions;
using Application.DTOs;

namespace Application.Services
{
    public class ChatQueryService : IChatQueryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatQueryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ConversationMessagesDTO> GetConversationMessagesAsync(string conversationId, string? next)
        {
            const int PageSize = 20;

            var messagesPlusOne = (await _unitOfWork.Messages.GetMessagesPageAsync(
                conversationId,
                next,
                PageSize + 1
            )).ToList();

            var messages = messagesPlusOne.Take(PageSize).ToList();
            next = messagesPlusOne.Count > PageSize ? messagesPlusOne[PageSize].Id : null;

            return new ConversationMessagesDTO
            {
                Messages = messages,
                Next = next
            };
        }

        public async Task<UserConversationsDTO> GetUserConversationsAsync(string userId, string? next)
        {
            const int PageSize = 20;

            var conversationsPlusOne = (await _unitOfWork.Conversations.GetConversationsPageAsync(
                userId,
                next,
                PageSize + 1
            )).ToList();

            var conversations = conversationsPlusOne.Take(PageSize).ToList();
            next = conversationsPlusOne.Count > PageSize ? conversationsPlusOne[PageSize].Id : null;

            return new UserConversationsDTO
            {
                Conversations = conversations,
                Next = next
            };
        }
    }
}
