using Microsoft.AspNetCore.Mvc;

namespace learingASP.Common
{
    public static class ResultExtensions
    {
        // chat gpt said I should change the return type here from IActionResult to ObjectResult so I can use 
        // Task<ActionResult<T>> in the controller

        public static ObjectResult ToProblem(this ControllerBase controller, Result result)
        {
            var status = result.Error.Kind switch
            {
                ErrorKind.NotFound => StatusCodes.Status404NotFound,
                ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
                ErrorKind.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            };

            return controller.Problem(statusCode: status, detail: result.Error.Message);
        }
    }
}
