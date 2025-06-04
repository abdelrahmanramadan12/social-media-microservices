using Application.DTOs;
using Application.IServices;
using Domain.Entities;

namespace Application.Services
{
    public class ValidationService : IValidationService
    {
        public async Task<ValidationResult> ValidateNewPost(PostInputDTO post, string userId)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(userId))
            {
                result.Errors.Add("UserId is missing");
                result.ErrorType = ErrorType.UnAuthorized;
                return result;
            }
            result = ValidatePostContent(post);

            if (!result.IsValid)
                return result;

            if (post.HasMedia)
            {
                result = ValidatePostMedia(post);
                if (!result.IsValid)
                    return result;
            }

            return result;
        }
        public async Task<ValidationResult> ValidateUpdatePost(PostInputDTO postInputDto, Post post, string userId)
        {
            var result = new ValidationResult();

            if (postInputDto == null || string.IsNullOrEmpty(postInputDto.PostId))
            {
                result.Errors.Add("Invalid Operation! post doesn't exist or has been deleted");
                result.ErrorType = ErrorType.BadRequest;
                return result;
            }

            if (post.AuthorId != userId)
            {
                result.Errors.Add("Invalid Operation! you don't have the permission to edit this post!");
                result.ErrorType = ErrorType.UnAuthorized;
                return result;
            }

            result = ValidatePostContent(postInputDto);
            if (!result.IsValid)
                return result;

            if (postInputDto.HasMedia)
            {
                result = ValidatePostMedia(postInputDto);
                if (!result.IsValid)
                    return result;
            }

            return result;
        }
        // Utility Validators
        private ValidationResult ValidatePostContent(PostInputDTO post)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(post.Content))
                result.Errors.Add("Post can't be empty!");
            else if (post.Content.Length > 500)
                result.Errors.Add("Post can't have more than 500 characters!");

            if (!result.IsValid)
                result.ErrorType = ErrorType.BadRequest;

            return result;
        }
        private ValidationResult ValidatePostMedia(PostInputDTO post)
        {
            var result = new ValidationResult();
            if (post.Media == null)
            {
                result.Errors.Add("Invalid media or it has been corrupted. Try again later.");
                result.ErrorType = ErrorType.BadRequest;
            }
            if (post.Media.Count() > 4)
            {
                result.Errors.Add("Invalid Request! The maximum number of media items you can upload per post is four (4) items.");
                result.ErrorType = ErrorType.BadRequest;
            }
            return result;
        }
    }
}
