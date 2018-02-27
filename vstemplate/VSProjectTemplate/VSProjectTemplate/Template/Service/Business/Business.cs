using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace $ext_safeprojectname$.Service.Business
{
    public class $ext_safeprojectname$Business : I$ext_safeprojectname$Business
    {
        private readonly IUnitOfWork _unitOfWork;

        public $ext_safeprojectname$Business(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
