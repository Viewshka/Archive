﻿using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Archive.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());

            return services;
        }

        // public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        //     IConfiguration configuration)
        // {
        //     
        //     return services;
        // }
    }
}