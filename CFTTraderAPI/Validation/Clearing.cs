using CFTTraderAPI.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CFTTraderAPI.Models;

namespace CFTTraderAPI.Validation
{
    public class Clearing: IClearing
    {        
        //Perform clearing
        public decimal CalculateClearing(DataTable dtTrades)
        {
            //if (dtTrades.Rows.Count > 0)
            //{
            //    List<Charge> lstCharge = new List<Charge>();
            //    foreach (DataRow dr in dtTrades.Rows)
            //    {
            //        Charge ch = new Charge();

            //        ch.TradeValue = CalculateTradeValue(decimal price, Guid commodityGrade, decimal soldQty, decimal lotSize);
            //        ch.LocationDifferential;
            //        ch.AdjustedPrice;
            //            ch.ActualWeight;
            //        ch.MoistureLoss =0;
            //        ch.AdjustedTradeValue;
            //        ch.VAT;
            //            ch.DeductVATONNOR;
            //        ch.TOT;
            //        ch.WT; 
            //        ch.TradeFee; 
            //        ch.HandlingCharge;
            //        ch.StorageCharge;
            //        ch.CoffeeSportClub;
            //        ch.OtherCharge;
            //        ch.IsIF;
            //        ch.IFFee;
            //        ch.ReBagging;
            //        ch.ClearingFee;
            //        ch.ECXVAT;
            //        ch.CounterPartyTINNumber;
            //        ch.CounterPartyVATNumber;
            //        ch.LotSize;
            //        ch.LotSizeInBag;
            //        ch.IsSell;
            //        ch. BagCost;
            //        ch.AdvancePaymentDeductibles;

            //    }
            //}
            return 0;
        }

        //Perform balance check

        public bool BalanceCheck()
        {
            return true;
        }
    }
}
