using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Models
{
    public class Charge
    {
        public string TicketNo
        {
            get;
            set;
        }
        public Int64 WHRNO
        {
            get;
            set;
        }
        public string Symbol
        {
            get;
            set;
        }
        public Guid CommodityGuid
        {
            get;
            set;
        }
        public Guid CommodityClassGuid
        {
            get;
            set;
        }
        public Guid CommodityGradeGuid
        {
            get;
            set;
        }
        public decimal Quantity
        {
            get;
            set;
        }
        public decimal Price
        {
            get;
            set;
        }
        public decimal TradeValue
        {
            get;
            set;
        }
        public decimal LocationDifferential
        {
            get;
            set;
        }
        public decimal AdjustedPrice
        {
            get;
            set;
        }
        public decimal ActualWeight
        {
            get;
            set;
        }
        public decimal MoistureLoss
        {
            get;
            set;
        }
        public decimal AdjustedTradeValue
        {
            get;
            set;
        }
        public decimal? VAT
        {
            get;
            set;
        }
        public bool DeductVATONNOR
        {
            get;
            set;
        }
        public decimal? TOT
        {
            get;
            set;
        }
        public decimal? WT
        {
            get;
            set;
        }
        public decimal TradeFee
        {
            get;
            set;
        }
        public decimal HandlingCharge
        {
            get;
            set;
        }
        public decimal StorageCharge
        {
            get;
            set;
        }
        public decimal CoffeeSportClub
        {
            get;
            set;
        }


        public decimal OtherCharge
        {
            get;
            set;
        }
        public bool IsIF
        {
            get;
            set;
        }
        public decimal IFFee
        {
            get;
            set;
        }
        public decimal ReBagging
        {
            get;
            set;
        }
        public decimal ClearingFee
        {
            get;
            set;
        }
        public decimal ECXVAT
        {
            get;
            set;
        }

        public string CounterPartyTINNumber
        {
            get;
            set;
        }
        public string CounterPartyVATNumber
        {
            get;
            set;
        }
        public decimal LotSize
        {
            get;
            set;
        }
        public decimal LotSizeInBag
        {
            get;
            set;
        }
        public bool IsSell
        {
            get;
            set;
        }
    
        public decimal BagCost
        {
            get;
            set;
        }
     
       
        public decimal AdvancePaymentDeductibles
        {
            get;
            set;
        }
    }
}
