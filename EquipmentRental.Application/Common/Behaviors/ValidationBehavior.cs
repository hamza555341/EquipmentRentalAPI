using EquipmentRental.Application.Common.Results;
using EquipmentRental.Application.Common.Results.Errors;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EquipmentRental.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
      where TRequest : IRequest<TResponse>

    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken))))
                .SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Any())
            {
                var errors = failures
                    .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
                    .ToList();

                return GenerateFailResult(errors);
            }

            return await next();
        }

        private TResponse GenerateFailResult(List<Error> errors)
        {
            var responseType = typeof(TResponse);

            // Non-generic Result
            if (responseType == typeof(Result))
                return (TResponse)(object)Result.Failure(errors);

            // Generic Result<T>
            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result))
            {
                var valueType = responseType.GetGenericArguments()[0];
                var resultGenericType = typeof(Result).MakeGenericType(valueType);
                var failMethod = resultGenericType.GetMethod("Fail", new Type[] { typeof(List<Error>) });
                if (failMethod == null)
                    throw new InvalidOperationException("Fail method not found on Result<T>.");

                var failResult = failMethod.Invoke(null, new object[] { errors });
                return (TResponse)failResult!;
            }

            throw new InvalidOperationException("Unsupported response type for ValidationBehavior.");
        }
    }
}
