using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Models
{
    public record ContractOrder
    {
        public int Id { get; set; }
        public string TicketNo { get; set; }
        public Guid CFTID { get; set; }
        public int TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public Guid CommodityGrade { get; set; }
        public Guid RepId { get; set; }
        public Guid SessionId { get; set; }
        public int OrderStatus { get; set; }
        public DateTime SubmitedTime { get; set; }
        public string RTC { get; set; }
        public Guid WHRId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
