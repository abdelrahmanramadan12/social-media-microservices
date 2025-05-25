using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IValidationService
    {
        public Task<ValidationResult> ValidateNewPost(PostInputDTO post, string userId);
        public Task<ValidationResult> ValidateUpdatePost(PostInputDTO postInputDto, Post post, string userId);
    }
}
