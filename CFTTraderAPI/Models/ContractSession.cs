using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Models
{
    public record ContractSession
    {
        public Guid ID { get; set; }
        public DateTime TradeDate { get; set; }
        public string SessionName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public DateTime? StartTmie { get; set; }
        public DateTime? EndTime { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
