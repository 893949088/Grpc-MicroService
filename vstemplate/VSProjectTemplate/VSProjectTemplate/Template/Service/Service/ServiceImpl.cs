using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Server;
using $ext_safeprojectname$.Service.Business;

namespace $ext_safeprojectname$.Service
{
    public class $ext_safeprojectname$ServiceImpl : $ext_safeprojectname$Service.$ext_safeprojectname$ServiceBase, IGrpcService
    {

        private readonly I$ext_safeprojectname$Business _business;

        public $ext_safeprojectname$ServiceImpl(I$ext_safeprojectname$Business business)
        {
            _business = business;
        }
    }
}
