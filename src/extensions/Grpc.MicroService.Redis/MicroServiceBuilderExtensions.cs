using Grpc.MicroService.Redis;
using Grpc.MicroService.Redis.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MicroServiceBuilderExtensions
    {
        public static IMicroServiceBuilder AddRedis(this IMicroServiceBuilder builder,string redisConnectionString)
        {
            builder.Services.AddSingleton<IRedisRepository>(p => new RedisRepository(redisConnectionString));
            return builder;
        }
    }
}
