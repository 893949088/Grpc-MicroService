using Example;
using ExampleServer.Business;
using Grpc.Core;
using Grpc.Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExampleServer
{
    public class ExampleServiceImpl : ExampleService.ExampleServiceBase, IGrpcService
    {
        private readonly IExampleBusiness _business;

        public ExampleServiceImpl(IExampleBusiness business)
        {
            _business = business;
        }

        public async override Task<AddUserReply> AddUser(AddUserArgs request, ServerCallContext context)
        {
            return await _business.AddUser(request);
        }
    }
}
