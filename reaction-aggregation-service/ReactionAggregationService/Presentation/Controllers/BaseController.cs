using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected ActionResult HandleErrorResponse<T>(ResponseWrapper<T> response)
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

        protected ActionResult HandlePaginationErrorResponse<T>(PaginationResponseWrapper<T> response)
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

        protected ActionResult HandlePaginationResponse<T>(PaginationResponseWrapper<T> response)
        {
            return Ok(new
            {
                data = response.Data,
                hasMore = response.HasMore,
                next = response.Next
            });
        }

        protected ActionResult HandleResponse<T>(ResponseWrapper<T> response)
        {
            return Ok(new
            {
                data = response.Data,
                message = response.Message
            });
        }
    }
}

