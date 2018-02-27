using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Grpc.Server;
using System;
using System.Collections.Generic;
using System.Text;
using $ext_safeprojectname$.Service.Business;

namespace $ext_safeprojectname$.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<I$ext_safeprojectname$Business, $ext_safeprojectname$Business>();

            services.AddMicroService(builder =>
            {
                builder.AddService<$ext_safeprojectname$ServiceImpl>();
                builder.AddMysql(Configuration.GetConnectionString("DbConnectionString"));
            });
        }

        public void Configure(IGrpcServer app)
        {
            app.UseMicroService();
        }
    }
}
