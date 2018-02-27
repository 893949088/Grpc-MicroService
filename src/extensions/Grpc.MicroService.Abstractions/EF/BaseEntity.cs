using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Grpc.MicroService.EF
{
    public abstract class BaseEntity
    {

    }

    public abstract class OptimistEntity : BaseEntity
    {

        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ConcurrencyCheck]
        public DateTime Optimist { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
