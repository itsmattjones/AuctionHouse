﻿using AuctionHouse.Application.Common.Middleware;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionHouse.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAutoMapper(typeof(DependencyInjection));
            serviceCollection.AddMediatR(typeof(DependencyInjection));
            serviceCollection.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            serviceCollection.AddLogging();

            return serviceCollection;
        }
    }
}
