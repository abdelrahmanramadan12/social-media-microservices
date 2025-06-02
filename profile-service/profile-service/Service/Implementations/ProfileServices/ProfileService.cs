using Service.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.IRepository;
using Service.Interfaces.MediaServices;
using Service.Interfaces.ProfileServices;
using Service.Interfaces.RabbitMqServices;
using Service.Mappers;
using Service.DTOs.Requests;
using Service.DTOs.Responses;

namespace Service.Implementations.ProfileServices
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IProfilePublisher _profilePublisher;
        private readonly IMediaServiceClient _mediaServiceClient;

        public ProfileService(IProfileRepository profileRepository, IProfilePublisher profilePublisher, IMediaServiceClient mediaServiceClient)
        {
            _profileRepository = profileRepository;
            _profilePublisher = profilePublisher;
            _mediaServiceClient = mediaServiceClient;
        }

        public async Task<ResponseWrapper<Profile>> GetByUserIdAsync(string userId)
        {
            var response = new ResponseWrapper<Profile>();

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Errors = new List<string> { "User ID cannot be null or empty." };
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            try
            {
                var profile = await _profileRepository.GetByUserIdAsync(userId);
                if (profile == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.ErrorType = ErrorType.NotFound;
                    return response;
                }

                response.Data = profile;
                response.Message = "Profile retrieved successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = new List<string> { $"Failed to retrieve profile: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
        }

        public async Task<ResponseWrapper<Profile>> GetByUserNameAsync(string userName)
        {
            var response = new ResponseWrapper<Profile>();

            if (string.IsNullOrWhiteSpace(userName))
            {
                response.Errors = new List<string> { "Username cannot be null or empty." };
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            try
            {
                var profile = await _profileRepository.GetByUserNameAsync(userName);
                if (profile == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.ErrorType = ErrorType.NotFound;
                    return response;
                }

                response.Data = profile;
                response.Message = "Profile retrieved successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = new List<string> { $"Failed to retrieve profile: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
        }

        public async Task<ResponseWrapper<SimpleUserDto>> GetByUserIdMinAsync(string userId)
        {
            var response = new ResponseWrapper<SimpleUserDto>();

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Errors = new List<string> { "User ID cannot be null or empty." };
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            try
            {
                var profile = await _profileRepository.GetByUserIdMinAsync(userId);
                if (profile == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.ErrorType = ErrorType.NotFound;
                    return response;
                }

                response.Data = ProfileMapper.ToSimpleUserDto(profile);
                response.Message = "Profile retrieved successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = new List<string> { $"Failed to retrieve profile: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
        }

        public async Task<ResponseWrapper<SimpleUserDto>> GetByUserNameMinAsync(string userName)
        {
            var response = new ResponseWrapper<SimpleUserDto>();

            if (string.IsNullOrWhiteSpace(userName))
            {
                response.Errors = new List<string> { "Username cannot be null or empty." };
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            try
            {
                var profile = await _profileRepository.GetByUserNameMinAsync(userName);
                if (profile == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.ErrorType = ErrorType.NotFound;
                    return response;
                }

                response.Data = ProfileMapper.ToSimpleUserDto(profile);
                response.Message = "Profile retrieved successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = new List<string> { $"Failed to retrieve profile: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
        }

        public async Task<ResponseWrapper<List<SimpleUserDto>>> GetUsersByIdsAsync(List<string> userIds)
        {
            var response = new ResponseWrapper<List<SimpleUserDto>>();

            if (userIds == null || !userIds.Any())
            {
                response.Errors = new List<string> { "User IDs cannot be null or empty." };
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            try
            {
                var profiles = await _profileRepository.GetUsersByIdsAsync(userIds);
                if (profiles == null || !profiles.Any())
                {
                    response.Errors = new List<string> { "No profiles found for the provided user IDs." };
                    response.ErrorType = ErrorType.NotFound;
                    return response;
                }

                response.Data = profiles.Select(ProfileMapper.ToSimpleUserDto).ToList();
                response.Message = "Profiles retrieved successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = new List<string> { $"Failed to retrieve profiles: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
        }

        public async Task<ResponseWrapper<Profile>> AddAsync(string userId, ProfileRequestDto profileRequestDto)
        {
            var response = new ResponseWrapper<Profile>();

            if (profileRequestDto == null)
            {
                response.Errors = new List<string> { "Profile data cannot be null." };
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Errors = new List<string> { "User ID cannot be null or empty." };
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            try
            {
                // Check for existing username
                if (!string.IsNullOrWhiteSpace(profileRequestDto.UserName))
                {
                    var existingByUserName = await _profileRepository.GetByUserNameAsync(profileRequestDto.UserName);
                    if (existingByUserName != null)
                    {
                        response.Errors = new List<string> { "Username already exists." };
                        response.ErrorType = ErrorType.Validation;
                        return response;
                    }
                }

                var profileEntity = MapToProfileEntity(profileRequestDto, userId);

                // Handle profile picture upload
                if (profileRequestDto.ProfilePic != null && profileRequestDto.ProfilePic.Length > 0)
                {
                    var mediaUploadResponse = await _mediaServiceClient.UploadMediaAsync(new MediaUploadRequestDto
                    {
                        File = profileRequestDto.ProfilePic,
                        MediaType = MediaType.IMAGE,
                        usageCategory = UsageCategory.ProfilePicture
                    });

                    if (!mediaUploadResponse.Success)
                    {
                        response.Errors = new List<string> { "Failed to upload profile picture." };
                        response.ErrorType = ErrorType.InternalServerError;
                        return response;
                    }

                    profileEntity.ProfileUrl = mediaUploadResponse.Data.Urls.FirstOrDefault();
                }

                // Handle cover picture upload
                if (profileRequestDto.CoverPic != null && profileRequestDto.CoverPic.Length > 0)
                {
                    var mediaUploadResponse = await _mediaServiceClient.UploadMediaAsync(new MediaUploadRequestDto
                    {
                        File = profileRequestDto.CoverPic,
                        MediaType = MediaType.IMAGE,
                        usageCategory = UsageCategory.CoverPicture
                    });

                    if (!mediaUploadResponse.Success)
                    {
                        response.Errors = new List<string> { "Failed to upload cover picture." };
                        response.ErrorType = ErrorType.InternalServerError;
                        return response;
                    }

                    profileEntity.CoverUrl = mediaUploadResponse.Data.Urls.FirstOrDefault();
                }

                await _profileRepository.AddAsync(profileEntity);

                // Publish event
                var profileEvent = CreateProfileEvent(profileEntity, ProfileEventType.ProfileAdded);
                await _profilePublisher.PublishAsync(profileEvent);

                response.Data = profileEntity;
                response.Message = "Profile created successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = new List<string> { $"Failed to create profile: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
        }

        public async Task<ResponseWrapper<Profile>> UpdateAsync(string userId, ProfileRequestDto profileRequestDto)
        {
            var response = new ResponseWrapper<Profile>();

            if (string.IsNullOrWhiteSpace(userId) || profileRequestDto == null)
            {
                response.Errors = new List<string> { "User ID and profile data cannot be null." };
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            try
            {
                var existingProfile = await _profileRepository.GetByUserIdAsync(userId);
                if (existingProfile == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.ErrorType = ErrorType.NotFound;
                    return response;
                }

                // Check for username uniqueness
                if (!string.IsNullOrWhiteSpace(profileRequestDto.UserName) &&
                    !string.Equals(existingProfile.UserName, profileRequestDto.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    var existingWithSameUserName = await _profileRepository.GetByUserNameAsync(profileRequestDto.UserName);
                    if (existingWithSameUserName != null && existingWithSameUserName.UserId != userId)
                    {
                        response.Errors = new List<string> { "Username is already taken by another user." };
                        response.ErrorType = ErrorType.Validation;
                        return response;
                    }
                }

                // Update fields
                existingProfile.UserName = profileRequestDto.UserName ?? existingProfile.UserName;
                existingProfile.FirstName = profileRequestDto.FirstName ?? existingProfile.FirstName;
                existingProfile.LastName = profileRequestDto.LastName ?? existingProfile.LastName;
                existingProfile.Bio = profileRequestDto.Bio ?? existingProfile.Bio;
                existingProfile.Address = profileRequestDto.Address ?? existingProfile.Address;
                if (profileRequestDto.BirthDate != default)
                    existingProfile.BirthDate = profileRequestDto.BirthDate;
                existingProfile.Email = profileRequestDto.Email ?? existingProfile.Email;
                existingProfile.MobileNo = profileRequestDto.MobileNo ?? existingProfile.MobileNo;

                // Handle profile picture update
                if (profileRequestDto.ProfilePic != null && profileRequestDto.ProfilePic.Length > 0)
                {
                    var mediaUploadResponse = await _mediaServiceClient.UploadMediaAsync(new MediaUploadRequestDto
                    {
                        File = profileRequestDto.ProfilePic,
                        MediaType = MediaType.IMAGE,
                        usageCategory = UsageCategory.ProfilePicture
                    });

                    if (!mediaUploadResponse.Success)
                    {
                        response.Errors = new List<string> { "Failed to upload profile picture." };
                        response.ErrorType = ErrorType.InternalServerError;
                        return response;
                    }

                    if (!string.IsNullOrWhiteSpace(existingProfile.ProfileUrl))
                    {
                        await _mediaServiceClient.DeleteMediaAsync(new List<string> { existingProfile.ProfileUrl });
                    }
                    existingProfile.ProfileUrl = mediaUploadResponse.Data.Urls.FirstOrDefault();
                }

                // Handle cover picture update
                if (profileRequestDto.CoverPic != null && profileRequestDto.CoverPic.Length > 0)
                {
                    var mediaUploadResponse = await _mediaServiceClient.UploadMediaAsync(new MediaUploadRequestDto
                    {
                        File = profileRequestDto.CoverPic,
                        MediaType = MediaType.IMAGE,
                        usageCategory = UsageCategory.CoverPicture
                    });

                    if (!mediaUploadResponse.Success)
                    {
                        response.Errors = new List<string> { "Failed to upload cover picture." };
                        response.ErrorType = ErrorType.InternalServerError;
                        return response;
                    }

                    if (!string.IsNullOrWhiteSpace(existingProfile.CoverUrl))
                    {
                        await _mediaServiceClient.DeleteMediaAsync(new List<string> { existingProfile.CoverUrl });
                    }
                    existingProfile.CoverUrl = mediaUploadResponse.Data.Urls.FirstOrDefault();
                }

                await _profileRepository.Update(existingProfile);

                // Publish event
                var profileEvent = CreateProfileEvent(existingProfile, ProfileEventType.ProfileUpdated);
                await _profilePublisher.PublishAsync(profileEvent);

                response.Data = existingProfile;
                response.Message = "Profile updated successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = new List<string> { $"Failed to update profile: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
        }

        public async Task<ResponseWrapper<bool>> DeleteAsync(string userId)
        {
            var response = new ResponseWrapper<bool>();

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Errors = new List<string> { "User ID cannot be null or empty." };
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            try
            {
                var existingProfile = await _profileRepository.GetByUserIdAsync(userId);
                if (existingProfile == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.ErrorType = ErrorType.NotFound;
                    return response;
                }

                await _profileRepository.DeleteAsync(userId);

                // Publish event
                var profileEvent = new ProfileEvent
                {
                    EventType = ProfileEventType.ProfileDeleted,
                    User = new ProfileEventData { UserId = userId }
                };
                await _profilePublisher.PublishAsync(profileEvent);

                response.Data = true;
                response.Message = "Profile deleted successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = new List<string> { $"Failed to delete profile: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
        }

        private Profile MapToProfileEntity(ProfileRequestDto dto, string userId)
        {
            return ProfileMapper.ToProfileEntity(dto, userId);
        }

        private ProfileEvent CreateProfileEvent(Profile profile, ProfileEventType eventType)
        {
            return new ProfileEvent
            {
                EventType = eventType,
                User = ProfileMapper.ToProfileEventData(profile)
            };
        }
    }
}
