using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Common.Results.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace EquipmentRental.Api.Controllers.Common
{
    [ApiController]
    [Route("api/[controller]")]

    public class ApiBaseController : ControllerBase
    {
        // handle result without Value
        // if Result Is Succes return No Content
        //  if Result Is Failure return  Problem With statesCode and Details
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return NoContent();// 204
            }
            else
                return HandleProblem(result.Errors);
        }

        // handle  result  With  Value
        // if Result Is Succes return Ok 200 With Value
        //if Result Is Failure return Problem With statesCode and Details
        protected ActionResult<TValue> HandleResult<TValue>(Result<TValue> result)
        {
            if (result.IsSuccess)
                return Ok(result.Value);

            else
                return HandleProblem(result.Errors);
        }



        private ActionResult HandleProblem(IReadOnlyList<Error> errors)

        {
            // if no errors , return 500 
            if (errors.Count == 0)
                return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "A unExpected Error Occured");
            //if all Errors is Validation , return ValidationProblem 
            if (errors.All(e => e.Type == ErrorType.Validation))
            {
                return HandleValidationProblem(errors);
            }

            // if has one error , handle SingleError Problem
            return HandleSingleErrorProblem(errors[0]);

        }

        private ActionResult HandleValidationProblem(IReadOnlyList<Error> errors)
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in errors)
            {
                modelState.AddModelError(error.Code, error.Description);

            }

            return ValidationProblem(modelState);

        }

        private ActionResult HandleSingleErrorProblem(Error error)
        {
            return Problem(title: error.Code,
                detail: error.Description,
                type: error.Type.ToString(),
                 statusCode: MapErrortypeToStatusCode(error.Type));

        }

        private static int MapErrortypeToStatusCode(ErrorType errorType) => errorType switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.InValidCerdentials => StatusCodes.Status401Unauthorized,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError,


        };

        protected string GetEmailFromToken() => User.FindFirstValue(ClaimTypes.Email)!;


    }
}
