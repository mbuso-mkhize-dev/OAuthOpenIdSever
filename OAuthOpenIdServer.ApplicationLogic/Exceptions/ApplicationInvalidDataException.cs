using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OAuthOpenIdServer.ApplicationLogic.Exceptions
{
    public class ApplicationInvalidDataException : Exception
    {
        //private readonly ModelStateDictionary _modelState;

        private readonly IEnumerable<IdentityError> _identityErrors;

        public ApplicationInvalidDataException()
        {
        }

        public ApplicationInvalidDataException(string message) : base(message)
        {
        }

        public ApplicationInvalidDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        //public ApplicationInvalidDataException(ModelStateDictionary modelState) : this(GetExceptionMessageFromModelState(modelState))
        //{
        //    _modelState = modelState;
        //}

        public ApplicationInvalidDataException(IEnumerable<IdentityError> identityErrors) : this(GetExceptionMessageFromIdentityErrors(identityErrors))
        {
            _identityErrors = identityErrors;
        }

        //private static string GetExceptionMessageFromModelState(ModelStateDictionary modelState)
        //{
        //    var messages = new List<string>();

        //    foreach (var key in modelState.Keys)
        //    {
        //        messages.AddRange(modelState[key].Errors.Select(m => m.ErrorMessage));
        //    }

        //    return string.Join(" ", messages);
        //}

        private static string GetExceptionMessageFromIdentityErrors(IEnumerable<IdentityError> identityErrors)
        {
            var messages = new List<string>();

            if (identityErrors != null && identityErrors.Any())
            {
                foreach (var identityError in identityErrors)
                {
                    messages.Add(identityError.Description);
                }
            }

            return string.Join(" ", messages);
        }
    }
}