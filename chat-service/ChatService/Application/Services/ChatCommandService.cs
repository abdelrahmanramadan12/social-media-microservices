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

        public async Task DeleteConversationAsync(string userId, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id), "Conversation ID cannot be null or empty.");
            }
            var conversation =  await _unitOfWork.Conversations.GetConversationByIdAsync(id);
            if (conversation == null)
            {
                throw new ArgumentNullException(nameof(conversation));
            }
            // Check if the conversation is a group conversation and the user is an admin
            // Temporary check for group

            if (!conversation.IsGroup )
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this group conversation");
            }
            if ( userId != conversation.AdminId)
            {
                throw new UnauthorizedAccessException("You are not authorized to remove this message"); 
            }

            await _unitOfWork.Conversations.RemoveAsync(id);
        }

        public async Task DeleteMessageAsync(string messageId)
        {
            await _unitOfWork.Messages.RemoveAsync(messageId);
        }

        public async Task<MessageDTO> EditMessageAsync(MessageDTO message)
        {
            if (message == null || string.IsNullOrEmpty(message.Id))
            {
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");
            }
            var existingMessage = await _unitOfWork.Messages.GetByIdAsync(message.Id);
            if (existingMessage == null) {
                throw new KeyNotFoundException($"Message with ID {message.Id} not found.");
            }

            if(existingMessage.SenderId != message.SenderId)
            {
                throw new UnauthorizedAccessException("You are not authorized to edit this message.");
            }

            existingMessage.Text = !string.IsNullOrEmpty(message.Content) ? message.Content : existingMessage.Text;
            existingMessage.EditedAt = DateTime.UtcNow;
            existingMessage.IsEdited = true;

            var msg= await _unitOfWork.Messages.EditAsync(existingMessage);
            if (!msg)
            {
                throw new Exception("Failed to edit message. Please try again.");
            }
            return new MessageDTO
            {
                Id = existingMessage.Id,
                ConversationId = existingMessage.ConversationId,
                SenderId = existingMessage.SenderId,
                Content = existingMessage.Text,
            };
        }

        public async Task<MessageDTO> SendMessageAsync(NewMessageDTO message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");
            }
            if (string.IsNullOrEmpty(message.ConversationId) || string.IsNullOrEmpty(message.SenderId) )
            {
                throw new ArgumentNullException("Message must have a valid ConversationId and SenderId");
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

            var msg = await _unitOfWork.Messages.AddAsync(messageEntity);

            return new MessageDTO
            {
                Id = msg.Id,
                ConversationId = msg.ConversationId,
                SenderId = msg.SenderId,
                Content = msg.Text,
                
            };

        }
    }
}
