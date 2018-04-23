using Grpc.MicroService.EF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExampleServer.Domain
{
    [Table("User")]
    public class User : OptimistEntity
    {
        [Key]
        public int UserId { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }
    }
}
