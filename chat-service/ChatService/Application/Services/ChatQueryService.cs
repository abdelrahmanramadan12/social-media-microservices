using Application.Abstractions;
using Application.DTOs;
using Domain.Entities;

namespace Application.Services
{
    public class ChatQueryService : IChatQueryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatQueryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ConversationMessagesDTO> GetConversationMessagesAsync(string userId, string conversationId, string? next , int pageSize)
        {
      

            var messagesPlusOne = (await _unitOfWork.Messages.GetMessagesPageAsync(
                conversationId,
                next,
                pageSize + 1
            )).ToList();

            var messages = messagesPlusOne.Take(pageSize).ToList();
            next = messagesPlusOne.Count > pageSize ? messagesPlusOne[pageSize].Id : null;

            return new ConversationMessagesDTO
            {
                Messages = messages?.Select(m => new MessageDTO
                {
                    Id = m.Id,
                    Content = m.Text,
                    SenderId = m.SenderId,
                    ConversationId = m.ConversationId,
                    HasAttachment = m.Attachment != null,
                    Attachment = new Attachment
                    {
                        Url = m.Attachment?.Url ?? string.Empty,
                        Type = m.Attachment?.Type ?? Domain.Enums.MediaType.Image
                    },
                    Read = m.ReadBy.Keys.Contains(userId)
                }).ToList(),
                Next = next
            };
        }

        public async Task<UserConversationsDTO> GetUserConversationsAsync(string userId, string? next, int pageSize)
        {

            var conversationsPlusOne = (await _unitOfWork.Conversations.GetConversationsAsync(
                userId,
                next,
                pageSize + 1
            )).ToList();

            var conversations = conversationsPlusOne.Take(pageSize).ToList();
            next = conversationsPlusOne.Count > pageSize ? conversationsPlusOne[pageSize].Id : null;

            return new UserConversationsDTO
            {
                Conversations = conversations?.Select(c => new ConversationDTO
                {
                    Id = c.Id,
                    CreatedAt = c.CreatedAt,
                    GroupName = c.GroupName,
                    IsGroup = c.IsGroup,
                    GroupImageUrl = c.GroupImageUrl,
                    AdminId = c.AdminId,
                    LastMessage = c.LastMessage != null ? new MessageDTO()
                    {
                        Id = c.LastMessage.Id,
                        Content = c.LastMessage.Text,
                        ConversationId = c.Id,
                        SenderId = c.LastMessage.SenderId,
                        HasAttachment = c.LastMessage.Attachment != null,
                        Read = c.LastMessage.ReadBy.Keys.Contains(userId)
                    } : null,
                    Participants = c.Participants
                }).ToList(),
                Next = next
            };
        }
    }
}
