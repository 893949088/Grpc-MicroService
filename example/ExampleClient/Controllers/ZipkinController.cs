using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Example;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExampleClient.Controllers
{
    [Produces("application/json")]
    [Route("api/Zipkin")]
    public class ZipkinController : Controller
    {
        private readonly ExampleService.ExampleServiceClient _exampleClient;


        public ZipkinController(
            ExampleService.ExampleServiceClient exampleClient)
        {
            _exampleClient = exampleClient;
        }

        [Route("AddUser")]
        [HttpGet]
        public async Task<IActionResult> AddUser(string name = "yhh")
        {
            return Ok(await _exampleClient.AddUserAsync(new AddUserArgs { Name = name }));
        }
    }
}