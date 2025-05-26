using Application.DTOs;
using Domain.Entities;

namespace Application.IServices
{
    public interface IValidationService
    {
        public Task<ValidationResult> ValidateNewPost(PostInputDTO post, string userId);
        public Task<ValidationResult> ValidateUpdatePost(PostInputDTO postInputDto, Post post, string userId);
    }
}
