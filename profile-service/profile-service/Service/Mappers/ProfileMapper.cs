using Domain.Entities;
using Domain.Events;
using Service.DTOs;

namespace Service.Mappers
{
    public static class ProfileMapper
    {
        public static SimpleUserDto ToSimpleUserDto(Profile profile)
        {
            if (profile == null) return null;
            
            return new SimpleUserDto
            {
                UserId = profile.UserId,
                DisplayName = $"{profile.FirstName} {profile.LastName}".Trim(),
                UserName = profile.UserName,
                ProfilePictureUrl = profile.ProfileUrl
            };
        }

        public static Profile ToProfileEntity(ProfileRequestDto dto, string userId)
        {
            if (dto == null) return null;
            
            return new Profile
            {
                UserId = userId,
                UserName = dto.UserName ?? $"@User{Guid.NewGuid()}",
                FirstName = dto.FirstName ?? "New",
                LastName = dto.LastName ?? "User",
                Bio = dto.Bio,
                Address = dto.Address,
                BirthDate = dto.BirthDate == default ? DateTime.Now.AddYears(-20) : dto.BirthDate,
                Email = dto.Email,
                MobileNo = dto.MobileNo,
                NoFollowers = 0,
                NoFollowing = 0
            };
        }

        public static ProfileEventData ToProfileEventData(Profile profile)
        {
            if (profile == null) return null;

            return new ProfileEventData
            {
                UserId = profile.UserId,
                UserName = profile.UserName,
                DisplayName = $"{profile.FirstName} {profile.LastName}".Trim(),
                ProfilePictureUrl = profile.ProfileUrl ?? " "
            };
        }

        public static SimpleUserDto ToSimpleUserDto(ProfileEventData eventData)
        {
            if (eventData == null) return null;

            return new SimpleUserDto
            {
                UserId = eventData.UserId,
                UserName = eventData.UserName,
                DisplayName = eventData.DisplayName,
                ProfilePictureUrl = eventData.ProfilePictureUrl
            };
        }
    }
} 