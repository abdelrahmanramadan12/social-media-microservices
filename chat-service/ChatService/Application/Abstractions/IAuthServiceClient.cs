using Application.DTOs;

namespace Application.Abstractions
{
    public interface IAuthServiceClient
    {
        Task<Response<AuthResponseDTO>> VerifyTokenAsync(string token);
    }
}
