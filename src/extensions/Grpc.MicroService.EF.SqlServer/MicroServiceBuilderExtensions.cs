using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MicroServiceBuilderExtensions
    {
        public static IMicroServiceBuilder AddMysql(this IMicroServiceBuilder builder,string dbConnectionString)
        {
            builder.Services.AddDbContextPool<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));
            builder.Services.AddUnitOfWork<ApplicationDbContext>();
            return builder;
        }
    }
}
