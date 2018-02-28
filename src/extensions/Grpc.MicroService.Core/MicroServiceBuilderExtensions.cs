using Grpc.MicroService.EF;
using Grpc.MicroService.Redis;
using Grpc.Server;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MicroServiceBuilderExtensions
    {
        public static IMicroServiceBuilder RegisterService<T>(this IMicroServiceBuilder builder) where T : class, IGrpcService
        {
            builder.Services.AddScoped<T>();
            builder.Services.AddSingleton<IGrpcService, T>();

            return builder;
        }

        public static IMicroServiceBuilder AddMysql(this IMicroServiceBuilder builder, string dbConnectionString)
        {
            builder.Services.AddDbContextPool<ApplicationDbContext>(options => options.UseMySQL(dbConnectionString));
            builder.Services.AddUnitOfWork<ApplicationDbContext>();
            return builder;
        }

        public static IMicroServiceBuilder AddRedis(this IMicroServiceBuilder builder, string redisConnectionString)
        {
            builder.Services.AddSingleton<IRedisRepository>(p => new StackExchangeRedisRespository(redisConnectionString));
            return builder;
        }
        
    }
}
