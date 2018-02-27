using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Example;
using ExampleServer.Domain;
using Microsoft.EntityFrameworkCore;

namespace ExampleServer.Business
{
    public class ExampleBusiness : IExampleBusiness
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExampleBusiness(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<AddUserReply> AddUser(AddUserArgs args)
        {
            var user = _unitOfWork.GetRepository<User>();
            user.Insert(new User
            {
                Name = args.Name
            });

            await _unitOfWork.SaveChangesAsync();

            return new AddUserReply
            {
                Success = true,
                Code = "10000"
            };
        }
    }
}
