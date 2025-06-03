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

        public async Task CreateConversationAsync(NewConversationDTO conversation)
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

            await _unitOfWork.Conversations.AddAsync(conversationEntity);
        }

        public async Task DeleteMessageAsync(string messageId)
        {
            if (string.IsNullOrEmpty(messageId))
            {
                throw new ArgumentException("Message ID cannot be null or empty.", nameof(messageId));
            }
            var message = await _unitOfWork.Messages.GetByIdAsync(messageId);

            if (message == null)
            {
                return; 
            }
            await _unitOfWork.Messages.RemoveAsync(message);
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
