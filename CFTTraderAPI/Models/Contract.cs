using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Models
{
    public record Contract
    {
        public Guid ID { set; get; }
        public string TicketNo { set; get; }

        public Guid CFTID { set; get; }

        public int TransactionType { set; get; }

        public decimal Quantity { set; get; }

        public decimal Price { set; get; }

        public Guid CommodityGrade { set; get; }

        public Guid RepId { set; get; }

        public Guid SessionId { set; get; }

        public int OrderStatus { set; get; }

        public DateTime SubmitedTime { set; get; }

        public string RTC { set; get; }

        public Guid WHRId { set; get; }

        public Guid CreatedBy { set; get; }
        public Guid UpdatedBy { set; get; }
        public DateTime UpdatedDate { set; get; }
    }
}
