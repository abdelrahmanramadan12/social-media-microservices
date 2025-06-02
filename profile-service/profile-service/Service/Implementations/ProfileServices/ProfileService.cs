using Service.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.IRepository;
using Service.Interfaces.MediaServices;
using Service.Interfaces.ProfileServices;
using Service.Interfaces.RabbitMqServices;
using Service.Mappers;

namespace Service.Implementations.ProfileServices
{
    public class ProfileService :IProfileService
    {

        private readonly IProfileRepository _profileRepository;
        private readonly IProfilePublisher _profilePublisher;
        private readonly IMediaServiceClient _mediaServiceClient;
        public ProfileService(IProfileRepository profileRepository ,IProfilePublisher profilePublisher , IMediaServiceClient mediaServiceClient)
        {
            _profileRepository = profileRepository;
            _profilePublisher = profilePublisher;
            _mediaServiceClient = mediaServiceClient;
        }

        public async Task<ProfileResponseDto?> GetByUserIdAsync(string userId)
        {
            var response = new ProfileResponseDto
            {
                Success = true,
                Errors = null,
                Data = null
            };

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Errors = new List<string> { "User ID cannot be null or empty." };
                response.Success = false;

            }
            else
            {

                var profile = await _profileRepository.GetByUserIdAsync(userId);
                if (profile == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.Success = false;
                }
                else
                {
                    response.Data = profile;
                }
            }
            return response;

        }


        public async Task<ProfileResponseDto?> GetByUserNameAsync(string userName)
        {
            var response = new ProfileResponseDto
            {
                Success = true,
                Errors = null,
                Data = null
            };
            if (string.IsNullOrWhiteSpace(userName))
            {
                response.Errors = new List<string> { "User name cannot be null or empty." };
                response.Success = false;
            }
            else
            {
                var profile = await _profileRepository.GetByUserNameAsync(userName);
                if (profile == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.Success = false;
                }
                else
                {
                    response.Data = profile;
                }
            }
            return response;

        }


        public async Task<MinProfileResponseDto?> GetByUserIdMinAsync(string userId)
        {
            var response = new MinProfileResponseDto
            {
                Success = true,
                Errors = null,
                Data = null
            };
            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Errors = new List<string> { "User ID cannot be null or empty." };
                response.Success = false;
            }
            else
            {
                var profile = await _profileRepository.GetByUserIdMinAsync(userId);
                if (profile == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.Success = false;
                }
                else
                {
                    response.Data = ProfileMapper.ToSimpleUserDto(profile);
                }
            }
            return response;
        }

        public async Task<MinProfileResponseDto?> GetByUserNameMinAsync(string userName)
        {
            var response = new MinProfileResponseDto
            {
                Success = true,
                Errors = null,
                Data = null
            };
            if (string.IsNullOrWhiteSpace(userName))
            {
                response.Errors = new List<string> { "User name cannot be null or empty." };
                response.Success = false;
            }
            else
            {
                var profile = await _profileRepository.GetByUserNameMinAsync(userName);
                if (profile == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.Success = false;
                }
                else
                {
                    response.Data = ProfileMapper.ToSimpleUserDto(profile);
                }
            }
            return response;
        }


        public async Task<ProfileListResponseDto?> GetUsersByIdsAsync(List<string> userIds)
        {
            var response = new ProfileListResponseDto
            {
                Success = true,
                Errors = null,
                Data = null
            };
            if (userIds == null || userIds.Count == 0)
            {
                response.Errors = new List<string> { "User IDs cannot be null or empty." };
                response.Success = false;
            }
            else
            {
                var profiles = await _profileRepository.GetUsersByIdsAsync(userIds);
                if (profiles == null || profiles.Count == 0)
                {
                    response.Errors = new List<string> { "No profiles found for the provided user IDs." };
                    response.Success = false;
                }
                else
                {
                    response.Data = profiles.Select(ProfileMapper.ToSimpleUserDto).ToList();
                }
            }
            return response;
        }

        public async Task<ProfileResponseDto?> AddAsync(string userId, ProfileRequestDto profileRequestDto)
        {
            var response = new ProfileResponseDto
            {
                Success = true,
                Errors = null,
                Data = null
            };

            if (profileRequestDto == null)
            {
                response.Errors = new List<string> { "Profile cannot be null." };
                response.Success = false;
                return response;
            }

            // Check for existing username
            if (!string.IsNullOrWhiteSpace(profileRequestDto.UserName))
            {
                var existingByUserName = await _profileRepository.GetByUserNameAsync(profileRequestDto.UserName);
                if (existingByUserName != null)
                {
                    response.Errors = new List<string> { "Username already exists." };
                    response.Success = false;
                    return response;
                }
            }

            // Map to Profile entity
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

                if (mediaUploadResponse.Success && mediaUploadResponse.Urls.Any())
                {
                    profileEntity.ProfileUrl = mediaUploadResponse.Urls.FirstOrDefault();
                }
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

                if (mediaUploadResponse.Success && mediaUploadResponse.Urls.Any())
                {
                    profileEntity.CoverUrl = mediaUploadResponse.Urls.FirstOrDefault();
                }
            }

            await _profileRepository.AddAsync(profileEntity);
            response.Data = profileEntity;

            // Publish event
            var profileEvent = CreateProfileEvent(profileEntity, ProfileEventType.ProfileAdded);
            await _profilePublisher.PublishAsync(profileEvent);

            return response;
        }


        public async Task<ProfileResponseDto?> UpdateAsync(string userId, ProfileRequestDto profileRequestDto)
        {
            var response = new ProfileResponseDto
            {
                Success = true,
                Errors = null,
                Data = null
            };

            if (string.IsNullOrWhiteSpace(userId) || profileRequestDto == null)
            {
                response.Errors = new List<string> { "User ID and profile cannot be null or empty." };
                response.Success = false;
                return response;
            }

            var existingProfile = await _profileRepository.GetByUserIdAsync(userId);
            if (existingProfile == null)
            {
                response.Errors = new List<string> { "Profile not found." };
                response.Success = false;
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
                    response.Success = false;
                    return response;
                }
            }

            // Update only provided fields
            existingProfile.UserName = profileRequestDto.UserName ?? existingProfile.UserName;
            existingProfile.FirstName = profileRequestDto.FirstName ?? existingProfile.FirstName;
            existingProfile.LastName = profileRequestDto.LastName ?? existingProfile.LastName;
            existingProfile.Bio = profileRequestDto.Bio ?? existingProfile.Bio;
            existingProfile.Address = profileRequestDto.Address ?? existingProfile.Address;
            if (profileRequestDto.BirthDate != default(DateTime))
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

                if (mediaUploadResponse.Success && mediaUploadResponse.Urls.Any())
                {
                    if (!string.IsNullOrWhiteSpace(existingProfile.ProfileUrl))
                    {
                        await _mediaServiceClient.DeleteMediaAsync(new List<string> { existingProfile.ProfileUrl });
                    }
                    existingProfile.ProfileUrl = mediaUploadResponse.Urls.FirstOrDefault();
                }
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

                if (mediaUploadResponse.Success && mediaUploadResponse.Urls.Any())
                {
                    if (!string.IsNullOrWhiteSpace(existingProfile.CoverUrl))
                    {
                        await _mediaServiceClient.DeleteMediaAsync(new List<string> { existingProfile.CoverUrl });
                    }
                    existingProfile.CoverUrl = mediaUploadResponse.Urls.FirstOrDefault();
                }
            }

            await _profileRepository.Update(existingProfile);
            response.Data = existingProfile;

            // Publish event
            var profileEvent = CreateProfileEvent(existingProfile, ProfileEventType.ProfileUpdated);
            await _profilePublisher.PublishAsync(profileEvent);

            return response;
        }


        public async Task<bool> DeleteAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var existingProfile = await _profileRepository.GetByUserIdAsync(userId);
            if (existingProfile == null)
            {
                return false;
            }
            await _profileRepository.DeleteAsync(userId);

            // Publish the profile Deleted event
            var profileEvent = new ProfileEvent
            {
                EventType = ProfileEventType.ProfileDeleted,
                User = new ProfileEventData
                {
                    UserId = userId
                }
            };
            await _profilePublisher.PublishAsync(profileEvent);
            return true;
        }


        // helper methods to handle profile creation and update events


        // Mapping methods to convert DTOs to entities and create events
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
