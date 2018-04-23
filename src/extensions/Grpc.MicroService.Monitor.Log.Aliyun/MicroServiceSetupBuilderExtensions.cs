using Grpc.MicroService.Internal;
using Grpc.MicroService.Log;
using Microsoft.Extensions.Configuration;
using System;

namespace Grpc.Server
{
    public static class MicroServiceSetupBuilderExtensions
    {
        public static IMicroServiceSetupBuilder UseAliyunLog(this IMicroServiceSetupBuilder config, IConfigurationSection aliyunLogSection)
        {
            var endpoint = aliyunLogSection.GetValue<string>("EndPoint");
            var accessKeyId = aliyunLogSection.GetValue<string>("AccessKeyId");
            var accessKeySecret = aliyunLogSection.GetValue<string>("AccessKeySecret");

            config.AddNLogRule(new EntityFrameworkTarget(endpoint, accessKeyId, accessKeySecret), "Microsoft.EntityFrameworkCore.*", true);
            config.AddNLogRule(new DefaultTarget(endpoint, accessKeyId, accessKeySecret), "AliyunLogger");

            config.Server.UseInterceptor(new GrpcMethodCallLogInterceptor(config.Server.ApplicationServices));
            return config;
        }
    }
}
