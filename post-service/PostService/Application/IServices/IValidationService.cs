using Application.DTOs;
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
    }
}
