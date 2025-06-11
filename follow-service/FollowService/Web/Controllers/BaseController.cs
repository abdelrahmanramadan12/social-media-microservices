using Application.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleError(ResponseWrapper response)
        {
            if (response == null)
            {
                return StatusCode(500, new { errors = new[] { "An unexpected error occurred" } });
            }

            return response.ErrorType switch
            {
                ErrorType.NotFound => NotFound(new { errors = response.Errors }),
                ErrorType.BadRequest => BadRequest(new { errors = response.Errors }),
                ErrorType.UnAuthorized => Unauthorized(new { errors = response.Errors }),
                ErrorType.Validation => UnprocessableEntity(new { errors = response.Errors }),
                ErrorType.InternalServerError => StatusCode(500, new { errors = response.Errors }),
                _ => BadRequest(new { errors = response.Errors })
            };
        }
    }
}