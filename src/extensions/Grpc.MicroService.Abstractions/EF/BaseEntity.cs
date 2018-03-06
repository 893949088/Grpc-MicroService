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
        public long Optimist { get; set; }

        public long CreateTime { get; set; }

        public long UpdateTime { get; set; }
    }
}
