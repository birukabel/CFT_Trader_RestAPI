using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECX.DATA
{
   public record Contract
    {
        public Guid ID { get; set; }
        public String ContractNumber { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public Guid CFTBuyId { get; set; }
        public Guid CFTSellId { get; set; }
        public int OptionId { get; set; }
        public int StatusId { get; set; }
        public Guid CommodityClassId { get; set; }
        public string Symbol { get; set; }
        public Guid ECXWarehouseId { get; set; }
        public string TraderWarehouse { get; set; }
        public int ProductionYear { get; set; }
        public decimal QuantityInLot { get; set; }
        public decimal QuantityNetWeight { get; set; }
        public decimal RemainingQuantity { get; set; }
        public string Price { get; set; }
        public string Remark { get; set; }
        public string Attachement { get; set; }
        public Guid MakerId { get; set; }
        public DateTime MakerDate { get; set; }
        public Guid CheckerId { get; set; }
        public DateTime CheckerDate { get; set; }

    }
}
