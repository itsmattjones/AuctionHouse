﻿using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuctionHouse.Application.Common.Middleware
{
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<TRequest> _logger;

        public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<TRequest> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            var failures = _validators.Select(v => v.Validate(context))
                .SelectMany(result => result.Errors).Where(f => f != null).ToList();

            if (failures.Any())
            {
                _logger.LogInformation($"Validation failures for request [{request}]");
                throw new ValidationException(failures);
            }

            return next();
        }
    }
}
