using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentRental.Application.Common.Results.Errors
{
    public class Error
    {
        public string Code { get; }
        public string Description { get; }
        public ErrorType Type { get; }


        private Error(string code, string description, ErrorType type)
        {
            Code = code;
            Description = description;
            Type = type;
        }


        // static factory Method To Create Error
        public static Error Failure(string code = "General.Failure", string description = " A General Error Has Occured")
        {
            return new Error(code, description, ErrorType.Failure);
        }

        public static Error Validation(string code = "General.Validation", string description = " A Validation Error Has Occured")
        {
            return new Error(code, description, ErrorType.Validation);
        }
        public static Error NotFound(string code = "General.NotFound", string description = "The Request Resource Was Not Found")
        {
            return new Error(code, description, ErrorType.NotFound);
        }
        public static Error Unauthorized(string code = "General.Unauthorized", string description = "You are not authorized to perform this action")
        {
            return new Error(code, description, ErrorType.Unauthorized);
        }
        public static Error Forbidden(string code = "General.Forbidden", string description = " You do not have permission to access this resource")
        {
            return new Error(code, description, ErrorType.Forbidden);
        }
        public static Error InValidCerdentials(string code = "General.InValidCerdentials", string description = " The provided credentials are invalid")
        {
            return new Error(code, description, ErrorType.InValidCerdentials);
        }

        public static Error Conflict(string code = "General.Conflict", string description = "A conflict has occurred")
        {
            return new Error(code, description, ErrorType.Conflict);
        }




    }
}
