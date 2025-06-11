using Microsoft.AspNetCore.Mvc;
using react_service.Application.DTO;
using System.ServiceModel.Channels;

namespace Web.Controllers
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
                Data = response.Data,
                HasMore = response.HasMore,
                Next = response.Next,
                Message = response.Message
            });
        }

        protected ActionResult HandleResponse<T>(ResponseWrapper<T> response)
        {
            return Ok(new { Data = response.Data, Message = response.Message });
        }
    }
}