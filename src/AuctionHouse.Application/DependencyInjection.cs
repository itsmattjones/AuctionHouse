namespace AuctionHouse.Application;

using AuctionHouse.Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

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