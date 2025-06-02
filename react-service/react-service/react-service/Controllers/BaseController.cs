using Microsoft.AspNetCore.Mvc;
using react_service.Application.DTO; 

namespace Web.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected ActionResult HandleServiceResponse<T>(ResponseWrapper<T> response)
        {
            if (response == null)
            {
                return StatusCode(500, new { errors = new[] { "An unexpected error occurred" } });
            }

            if (!response.Success)
            {
                var errorPayload = new { errors = response.Errors };
                return response.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(errorPayload),
                    ErrorType.BadRequest => BadRequest(errorPayload),
                    ErrorType.UnAuthorized => Unauthorized(errorPayload),
                    ErrorType.Validation => UnprocessableEntity(errorPayload),
                    ErrorType.InternalServerError => StatusCode(500, errorPayload),
                    _ => BadRequest(errorPayload)
                };
            }

            return Ok(new
            {
                data = response.Data,
                message = response.Message
            });
        }
    }
}