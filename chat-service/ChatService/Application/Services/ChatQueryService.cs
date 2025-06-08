using Application.Abstractions;
using Application.DTOs;
using Domain.Entities;

namespace Application.Services
{
    public class ChatQueryService : IChatQueryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProfileServiceClient _profileServiceClient;
        private readonly IProfileCache _profileCache;

        public ChatQueryService(IUnitOfWork unitOfWork, IProfileServiceClient profileServiceClient, IProfileCache profileCache)
        {
            _unitOfWork = unitOfWork;
            _profileServiceClient = profileServiceClient;
            _profileCache = profileCache;
        }

        public async Task<ConversationMessagesDTO> GetConversationMessagesAsync(string userId, string conversationId, string? next , int pageSize)
        {
            var messages = await GetPagedMessages(conversationId, next, pageSize);
            var dto = MapMessagesToDTO(messages.page, userId);
            await EnrichWithProfiles(dto);
            dto.Next = messages.next;
            return dto;
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

        private async Task<(List<Message> page, string? next)> GetPagedMessages(string convId, string? next, int size)
        {
            var messagesPlusOne = (await _unitOfWork.Messages.GetMessagesPageAsync(convId, next, size + 1)).ToList();
            var page = messagesPlusOne.Take(size).ToList();
            string? nextCursor = messagesPlusOne.Count > size ? messagesPlusOne[size].Id : null;
            return (page, nextCursor);
        }

        private ConversationMessagesDTO MapMessagesToDTO(List<Message> messages, string userId)
        {
            return new ConversationMessagesDTO
            {
                Messages = messages.Select(m => new MessageDTO
                {
                    Id = m.Id,
                    Content = m.Text,
                    SenderId = m.SenderId,
                    ConversationId = m.ConversationId,
                    HasAttachment = m.Attachment != null,
                    Attachment = m.Attachment != null ? new Attachment
                    {
                        Url = m.Attachment?.Url ?? string.Empty,
                        Type = m.Attachment?.Type ?? Domain.Enums.MediaType.Image
                    } : null,
                    Read = m.ReadBy?.ContainsKey(userId) == true
                }).ToList()
            };
        }

        private async Task EnrichWithProfiles(ConversationMessagesDTO dto)
        {
            var senderIds = dto.Messages.Select(m => m.SenderId).ToHashSet().ToList();

            var cachedProfiles = await _profileCache.GetProfilesAsync(senderIds);
            foreach (var msg in dto.Messages)
                msg.SenderProfile = cachedProfiles.FirstOrDefault(p => p.UserId == msg.SenderId);

            var notFound = dto.Messages.Where(m => m.SenderProfile == null).Select(m => m.SenderId).ToHashSet().ToList();
            if (notFound.Any())
            {
                var noncached = await _profileServiceClient.GetProfilesAsync(notFound);
                var dict = noncached.Data.ToDictionary(p => p.UserId, p => p);
                await _profileCache.AddProfilesAsync(dict);

                foreach (var msg in dto.Messages.Where(m => m.SenderProfile == null))
                    msg.SenderProfile = noncached.Data.FirstOrDefault(p => p.UserId == msg.SenderId);
            }
        }
    }
}
