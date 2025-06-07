using Application.Abstractions;
using Application.DTOs;
using Domain.Entities;

namespace Application.Services
{
    public class ChatCommandService : IChatCommandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatCommandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ConversationDTO> CreateConversationAsync(NewConversationDTO conversation)
        {
            var conversationEntity = new Conversation
            {
                Participants = conversation.Participants,
                IsGroup = conversation.IsGroup,
                GroupName = conversation.GroupName,
                CreatedAt = DateTime.UtcNow,
                LastMessage = null,
                AdminId = conversation.IsGroup ? conversation.UserId : null 
            };

            var conv = await _unitOfWork.Conversations.AddAsync(conversationEntity);

            return new ConversationDTO()
            {
                Id = conv.Id,
                CreatedAt = conv.CreatedAt,
                LastMessage = new MessageDTO()
                {
                    Id = conv.LastMessage.Id,
                    Content = conv.LastMessage.Text,
                    ConversationId = conv.Id,
                    SenderId = conv.LastMessage.SenderId
                },
                IsGroup = conv.IsGroup,
                Participants = conv.Participants,
                GroupName = conv.GroupName
            };
        }

        public async Task DeleteMessageAsync(string messageId)
        {
            await _unitOfWork.Messages.RemoveAsync(messageId);
        }

        public async Task EditMessageAsync(MessageDTO message)
        {
            if (message == null || string.IsNullOrEmpty(message.Id))
            {
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");
            }
            var existingMessage = await _unitOfWork.Messages.GetByIdAsync(message.Id);
            if (existingMessage == null) {
                throw new KeyNotFoundException($"Message with ID {message.Id} not found.");
            }

            existingMessage.Text = !string.IsNullOrEmpty(message.Content) ? message.Content : existingMessage.Text;
            existingMessage.EditedAt = DateTime.UtcNow;
            existingMessage.IsEdited = true;

            await _unitOfWork.Messages.AddAsync(existingMessage);
        }

        public Task SendMessageAsync(NewMessageDTO message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");
            }
            if (string.IsNullOrEmpty(message.ConversationId) || string.IsNullOrEmpty(message.SenderId) )
            {
                throw new ArgumentException("Message must have a valid ConversationId and SenderId");
            }

            var messageEntity = new Message
            { 
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                Text = !string.IsNullOrEmpty(message.Content)?message.Content:"",
                SentAt = DateTime.UtcNow,
            };
            if (message.Media != null)
            {
                // call a Service to handle file upload and set the attachment properties
            }

            return _unitOfWork.Messages.AddAsync(messageEntity);
        }
    }
}
