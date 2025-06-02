using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.IRepository;
using Service.Interfaces.MediaServices;
using Service.Interfaces.ProfileServices;
using Service.Interfaces.RabbitMqServices;

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
                var simpleUser = await _profileRepository.GetByUserIdMinAsync(userId);
                if (simpleUser == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.Success = false;
                }
                else
                {
                    response.Data = simpleUser;
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
                var simpleUser = await _profileRepository.GetByUserNameMinAsync(userName);
                if (simpleUser == null)
                {
                    response.Errors = new List<string> { "Profile not found." };
                    response.Success = false;
                }
                else
                {
                    response.Data = simpleUser;
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
                var simpleUsers = await _profileRepository.GetUsersByIdsAsync(userIds);
                if (simpleUsers == null || simpleUsers.Count == 0)
                {
                    response.Errors = new List<string> { "No profiles found for the provided user IDs." };
                    response.Success = false;
                }
                else
                {
                    response.Data = simpleUsers;
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
                User = new SimpleUserDto
                {
                    UserId = userId
                },

            };
            await _profilePublisher.PublishAsync(profileEvent);
            return true;
        }


        // helper methods to handle profile creation and update events


        // Mapping methods to convert DTOs to entities and create events
        private Profile MapToProfileEntity(ProfileRequestDto dto, string userId)
        {
            return new Profile
            {
                UserId = userId,
                UserName = dto.UserName ?? $"@User{Guid.NewGuid()}",
                FirstName = dto.FirstName ?? "New",
                LastName = dto.LastName ?? "User",
                Bio = dto.Bio,
                Address = dto.Address,
                BirthDate = dto.BirthDate== default ? DateTime.Now.AddYears(-20) : dto.BirthDate,
                Email = dto.Email,
                MobileNo = dto.MobileNo,
                NoFollowers = 0,
                NoFollowing = 0
            };
        }

        private ProfileEvent CreateProfileEvent(Profile profile, ProfileEventType eventType)
        {
            return new ProfileEvent
            {
                EventType = eventType,
                User = new SimpleUserDto
                {
                    UserId = profile.UserId,
                    UserName = profile.UserName,
                    DisplayName = profile.FirstName+" "+profile.LastName,
                    ProfilePictureUrl = profile.ProfileUrl ?? " "
                }
            };
        }
    }
}
