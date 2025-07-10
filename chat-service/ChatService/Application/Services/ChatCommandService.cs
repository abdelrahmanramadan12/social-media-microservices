using Application.Abstractions;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class ChatCommandService : IChatCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRealtimeMessenger _realtimeMessenger;
        private readonly IMediaServiceClient _mediaServiceClient;

        public ChatCommandService(IUnitOfWork unitOfWork, IRealtimeMessenger realtimeMessenger, IMediaServiceClient mediaServiceClient)
        {
            _unitOfWork = unitOfWork;
            _realtimeMessenger = realtimeMessenger;
            _mediaServiceClient = mediaServiceClient;
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

            conversationEntity.Participants.Add(conversation.UserId);
            
            if (conversation.GroupImage!=null)
            {
                var mediaResponse = await _mediaServiceClient.UploadMediaAsync(new MediaUploadRequest
                {
                    Files = new List<IFormFile> { conversation.GroupImage },
                    MediaType = Domain.Enums.MediaType.Image,
                    usageCategory= UsageCategory.ProfilePicture
                });
                if (mediaResponse != null && mediaResponse.Urls != null && mediaResponse.Urls.Any())
                {
                    conversationEntity.GroupImageUrl = mediaResponse.Urls.FirstOrDefault();  
                }
            }

            var conv = await _unitOfWork.Conversations.AddAsync(conversationEntity);

            return new ConversationDTO()
            {
                Id = conv.Id,
                CreatedAt = conv.CreatedAt,
                LastMessage = conv.LastMessage != null ? new MessageDTO()
                {
                    Id = conv.LastMessage.Id,
                    Content = conv.LastMessage.Text,
                    ConversationId = conv.Id,
                    SenderId = conv.LastMessage.SenderId
                } : null,
                IsGroup = conv.IsGroup,
                Participants = conv.Participants,
                GroupName = conv.GroupName
            };
        }
        public async Task<ConversationDTO> EditConversationAsync(EditConversationDTO conversation)
        {

            var ExistingConversation = await _unitOfWork.Conversations.GetConversationByIdAsync(conversation.Id);
            if (!ExistingConversation.IsGroup)
            {
                throw new UnauthorizedAccessException("You Can not edit this conversation.");
            }
            if (ExistingConversation == null)
            {
                throw new KeyNotFoundException($"Conversation with ID {conversation.Id} not found.");
            }
            if (ExistingConversation.IsGroup && ExistingConversation.AdminId != conversation.UserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to edit this group conversation.");
            }
            
            ExistingConversation.Participants=conversation.Participants== null ? ExistingConversation.Participants : conversation.Participants.ToList();
            ExistingConversation.GroupName = conversation.GroupName ?? ExistingConversation.GroupName;
            
            if (conversation.GroupImage != null)
            {
                var mediaResponse = await _mediaServiceClient.UploadMediaAsync(new MediaUploadRequest
                {
                    Files = new List<IFormFile> { conversation.GroupImage },
                    MediaType = MediaType.Image,
                    usageCategory= UsageCategory.ProfilePicture
                });
                if (mediaResponse != null && mediaResponse.Urls != null && mediaResponse.Urls.Any())
                {
                    ExistingConversation.GroupImageUrl = mediaResponse.Urls.FirstOrDefault();
                }
            }

            var updatedConversation = await _unitOfWork.Conversations.EditAsync(ExistingConversation);

            return new ConversationDTO {
                CreatedAt = updatedConversation.CreatedAt,
                GroupName = updatedConversation.GroupName,
                GroupImageUrl = updatedConversation.GroupImageUrl,
                Id = updatedConversation.Id,
                IsGroup = updatedConversation.IsGroup,
                AdminId = updatedConversation.AdminId,
                LastMessage = updatedConversation.LastMessage != null ? new MessageDTO
                {
                    Id = updatedConversation.LastMessage.Id,
                    Content = updatedConversation.LastMessage.Text,
                    ConversationId = updatedConversation.Id,
                    SenderId = updatedConversation.LastMessage.SenderId
                } : null,
                Participants = updatedConversation.Participants
            };

        }
        public async Task DeleteConversationAsync(string userId, string conversationId)
        {
            if (string.IsNullOrEmpty(conversationId))
            {
                throw new ArgumentNullException(nameof(conversationId), "Conversation ID cannot be null or empty.");
            }
            var conversation =  await _unitOfWork.Conversations.GetConversationByIdAsync(conversationId);
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
            if(conversation.GroupImageUrl != null)
            {
                // delete the group image if it exists
                await _mediaServiceClient.DeleteMediaAsync(new List<string> { conversation.GroupImageUrl });
            }

            await _unitOfWork.Conversations.RemoveAsync(conversationId);

            _ = Task.Run(async () =>
            {
                await _unitOfWork.Messages.DeleteMessagesByConversationIdAsync(conversationId);
            });
        }


        public async Task DeleteMessageAsync(string userId ,string messageId)
        {
            var message = await _unitOfWork.Messages.GetByIdAsync(messageId);
            if (message == null)
            {
                return;
            }
            var conversation = await _unitOfWork.Conversations.GetConversationByIdAsync(message.ConversationId);
            // Check if the message is sent by the user or if the user is an admin of the group conversation
            if (message.SenderId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to remove this message");
            }
            if (conversation.IsGroup && conversation.AdminId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to remove this message in a group conversation");
            }

            if (message.Attachment != null)
            {
                // delete the attachment if it exists
                await _mediaServiceClient.DeleteMediaAsync(new List<string> { message.Attachment.Url });
            }
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

            var msg = await _unitOfWork.Messages.EditAsync(existingMessage);
            if (msg == null)
            {
                throw new Exception("Failed to edit message. Please try again.");
            }
            return new MessageDTO
            {
                Id = msg.Id,
                ConversationId = msg.ConversationId,
                SenderId = msg.SenderId,
                Content = msg.Text,
                Read = true,
                HasAttachment = msg.Attachment != null,
                Attachment = msg.Attachment != null ? new Attachment
                {
                    Url = msg.Attachment.Url,
                    Type = msg.Attachment.Type
                } : null
            };
        }
        public async Task MarkReadAsync(string userId, string conversationId)
        {
            await _unitOfWork.Messages.MarkReadAsync(userId, conversationId);
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
                Text = !string.IsNullOrEmpty(message.Content) ? message.Content : "",
                ReadBy = new Dictionary<string, DateTime>
                {
                    { message.SenderId, DateTime.UtcNow }
                },
                SentAt = DateTime.UtcNow,
            };
            if (message.Media != null)
            {
                var mediaResponse = await _mediaServiceClient.UploadMediaAsync(new MediaUploadRequest
                {
                    Files = new List<IFormFile> { message.Media },
                    MediaType = (MediaType)message.MediaType!,
                });

                if (mediaResponse == null || mediaResponse.Urls == null || !mediaResponse.Urls.Any())
                {
                    throw new InvalidOperationException("Failed to upload media.");
                } else
                {
                    messageEntity.Attachment = new Attachment
                    {
                        Url = mediaResponse.Urls.FirstOrDefault(),
                        Type = (MediaType)message.MediaType!,
                    };
                }
            }

            var msg = await _unitOfWork.Messages.AddAsync(messageEntity);

            var msgDto = new MessageDTO
            {
                Id = msg.Id,
                ConversationId = msg.ConversationId,
                SenderId = msg.SenderId,
                Content = msg.Text,
                Read = true,
                Attachment = msg.Attachment != null ? new Attachment
                {
                    Url = msg.Attachment.Url,
                    Type = msg.Attachment.Type
                } : null,
                HasAttachment = msg.Attachment != null
            };

            var lstMsg = await _unitOfWork.Conversations.UpdateLastMessageAsync(msg);

            var conv = await _unitOfWork.Conversations.GetConversationByIdAsync(msg.ConversationId);

            if ( conv != null && conv.Participants != null)
            {
                foreach (var participant in conv.Participants.Except([msgDto.SenderId]))
                {
                    await _realtimeMessenger.SendMessageAsync(participant, msgDto);
                }
            }

            return msgDto;
        }
    }
}
