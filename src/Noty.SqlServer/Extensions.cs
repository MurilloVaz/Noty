﻿using Microsoft.Extensions.DependencyInjection;
using Noty.Interfaces;

namespace Noty.SqlServer
{
    public static class Extensions
    {
        public static void AddSqlServerContext(this IServiceCollection services, string connectionString)
        {
            services.AddScoped((x) => new SqlServerContext(connectionString));
        }

        public static void AddSqlServerContext(this IServiceCollection services, IContextConfiguration config)
        {
            services.AddScoped((x) => new SqlServerContext(config));
        }

    }
}
