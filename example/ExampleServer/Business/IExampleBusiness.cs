using Example;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExampleServer.Business
{
    public interface IExampleBusiness
    {
        Task<AddUserReply> AddUser(AddUserArgs args);
    }
}
