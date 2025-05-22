using Application.DTOs;
using Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ValidationService : IValidationService
    {
        public async Task<ValidationResult> ValidateNewPost(PostDTO post, string userId)
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
        public async Task<ValidationResult> ValidateUpdatePost(PostDTO post, string userId)
        {
            var result = new ValidationResult();

            if (post == null || string.IsNullOrEmpty(post.PostId))
            {
                result.Errors.Add("Post Id missing");
                result.ErrorType = ErrorType.BadRequest;
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

        // Utility Validators
        private ValidationResult ValidatePostContent(PostDTO post)
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
        private ValidationResult ValidatePostMedia(PostDTO post)
        {
            var result = new ValidationResult();
            if (post.Media == null)
                result.Errors.Add("Invalid media or it has been corrupted. try again later");
            result.ErrorType = ErrorType.BadRequest;

            return result;
        }

    }
}
