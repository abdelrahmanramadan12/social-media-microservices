using Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{

    namespace Application.Services
    {
        public class ProfileServiceClient : IProfileServiceClient
        {
            private readonly HttpClient _httpClient;

            public ProfileServiceClient(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            public async Task<ResponseWrapper<ProfileDTO>> GetProfileAsync(string userId)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"/api/internal/profile/min/id/{userId}");

                    if (!response.IsSuccessStatusCode)
                    {
                        return new ResponseWrapper<ProfileDTO>()
                        {

                            Data = null,
                            Errors = [$"Profile service returned status code {(int)response.StatusCode}: {response.ReasonPhrase}"]
                        };
                    }

                    var result = await response.Content.ReadFromJsonAsync<ResponseWrapper<ProfileDTO>>();

                    if (result == null)
                    {
                        return new ResponseWrapper<ProfileDTO>()
                        {
                            Data = null,
                            Errors = [$"Profile service returned a null or invalid response"]
                        };
                    }

                    return new ResponseWrapper<ProfileDTO>()
                    {
                        Data = result.Data,
                        Errors = []
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseWrapper<ProfileDTO>()
                    {
                        Data = null,
                        Errors = [$"Unhandled exception: {ex.Message}"]
                    };
                }
            }
        }
    }
}
