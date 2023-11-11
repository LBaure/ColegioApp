using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
using WebApi.Models;

namespace WebApi.Extensions
{
    public static class ValidationExceptionExtensions
    {
        public static ErrorModel MapToErrorModel(this ValidationException exception)
        {
            return new ErrorModel { Errors = ValidationFailuresToDictionary(exception.Errors) };
        }

        public static IDictionary<string, ICollection<string>> ValidationFailuresToDictionary(IEnumerable<ValidationFailure> failures)
        {

            var errors = new Dictionary<string, ICollection<string>>();

            foreach (var failure in failures)
            {
                var propertyName = ToLowerCammelCase(failure.PropertyName);
                var errorMessage = failure.ErrorMessage;
                AddErrorToDictionary(errors, propertyName, errorMessage);
            }

            return errors;
        }

        private static string ToLowerCammelCase(string propertyName)
        {
            if (propertyName.Length == 1) return propertyName.ToLowerInvariant();
            return propertyName.Substring(0, 1).ToLowerInvariant() + propertyName.Substring(1);
        }

        private static void AddErrorToDictionary(IDictionary<string, ICollection<string>> errors, string propertyName, string errorMessage)
        {
            if (!errors.ContainsKey(propertyName))
            {
                errors[propertyName] = new List<string>();
            }

            errors[propertyName].Add(errorMessage);
        }
    }
}
