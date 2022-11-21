using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Services
{
    public interface IClearingFunctions
    {
        public decimal CalculateTradeValue(decimal price, Guid whrId, decimal soldQty, decimal bagCost, Guid commodityGuid);

        public decimal CalculateLocationDifferential();

        public decimal CalculateAdjustedPrice(decimal price);

        public decimal CalculateActualWeight(Guid whrId, decimal soldQuantity);

        public decimal CalculateAdjustedTradeValue(decimal adjustedPrice, Guid whrId, decimal actualWeight, decimal moistureLoss);

        public decimal CalculateVAT(decimal adjustedTradeValue, Guid whrId, Guid ownerId, bool isMember);

        public decimal CalculateTOT(decimal adjustedTradeValue, Guid whrId, Guid ownerId, bool isMember);

        public decimal CalculateWT(decimal adjustedTradeValue, Guid ownerId, bool isMember);

        public decimal CalculateTradeFee(decimal price, Guid whrId, decimal soldQty, decimal bagCost);

        public decimal CalculateBagCost(int noOfBags);

        public decimal CalculateHandlingCharge(decimal soldQty, Guid whrId, bool isSell);

        public decimal CalculateStorageFee(decimal soldQTY, Guid whrId, DateTime trdaeDate);

        public decimal CalculateCoffeeSportClub(decimal soldQty, Guid whrId, Guid ownerId, bool isMember);

        public decimal CalculateRebaggingFee(Guid whrId, decimal soldQty);

        public decimal CalculateIFFee();

        public decimal CalculateClearingFee();

        public decimal CalculateOtherCharge(Guid whrId, decimal soldQty);

        public decimal CalculateECXVAT(Guid whrId, decimal price, decimal soldQty, DateTime tradeDate, bool isSell, int noOfBags);

        public decimal CalculateTotalNetObligation(bool isSellOrder, decimal adjustedTradeValue, decimal vat, decimal tot, decimal wt);
    }
}
