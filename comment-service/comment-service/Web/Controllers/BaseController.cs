using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Responses;

namespace Web.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected ActionResult HandleErrorResponse<T>(ResponseWrapper<T> response)
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

        protected ActionResult HandleResponse<T>(ResponseWrapper<T> response)
        {
            if (!response.Success)
            {
                return HandleErrorResponse(response);
            }

            return Ok(new
            {
                data = response.Data,
                message = response.Message
            });
        }

        protected ActionResult HandlePaginatedResponse<T>(ResponseWrapper<T> response)
        {
            if (!response.Success)
            {
                return HandleErrorResponse(response);
            }

            return Ok(new
            {
                data = response.Data,
                next = response?.Pagination?.Next,
                hasMore = response?.Pagination?.HasMore,
                message = response.Message
            });
        }

        protected ActionResult HandleCreatedResponse<T>(ResponseWrapper<T> response)
        {
            if (!response.Success)
            {
                return HandleErrorResponse(response);
            }

            return Created(string.Empty, new
            {
                data = response.Data,
                message = response.Message
            });
        }

        protected ActionResult HandleNoContentResponse<T>(ResponseWrapper<T> response)
        {
            if (!response.Success)
            {
                return HandleErrorResponse(response);
            }

            return NoContent();
        }
    }
}