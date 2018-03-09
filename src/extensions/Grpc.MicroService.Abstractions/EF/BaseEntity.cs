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

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [ConcurrencyCheck]
        public DateTime Optimist { get; set; }

        public long CreateTime { get; set; }

        public long UpdatedTime { get; set; }
    }
}
