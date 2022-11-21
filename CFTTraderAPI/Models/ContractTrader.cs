using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Models
{
    public record ContractTrader
    {
        public Guid ID { get; set; }
        public Guid MemberId { get; set; }
        public string MemberName { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public string TINNumber { get; set; }
        public string VATNumber { get; set; }
        public string Region { get; set; }
        public string Zone { get; set; }
        public string Woreda { get; set; }
        public string HouseNo { get; set; }
        public string Phone { get; set; }
        public bool IsSeller { get; set; }
    }
}
