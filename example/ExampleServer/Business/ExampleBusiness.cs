using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Example;
using ExampleServer.Domain;
using Grpc.MicroService.EF;
using Microsoft.Extensions.Logging;

namespace ExampleServer.Business
{
    public class ExampleBusiness : IExampleBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public ExampleBusiness(
            IUnitOfWork unitOfWork,
            ILoggerFactory loggerFactory)
        {
            _unitOfWork = unitOfWork;
            _logger = loggerFactory.CreateLogger("business");
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
