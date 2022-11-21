using CFTTraderAPI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Validation
{
    public class ClearingFunctions: IClearingFunctions
    {
        readonly IUtilities _iu;
        IConfiguration _cs;
        readonly ICFTOrderValidation _iov;
        readonly IContract _ic;
        public ClearingFunctions(IConfiguration cs, IUtilities iu, ICFTOrderValidation iov, IContract ic)
        {
            _cs = cs;
            _iu = iu;
            _iov = iov;
            _ic = ic;
        }

        // CFunctions.Round(((Order.Price / Convert.ToDecimal(this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid].PriceCalledPerKG))
        //* (Order.SoldQuantity * this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid].LotSize)), 2);
        public decimal CalculateTradeValue(decimal price, Guid whrId, decimal soldQty, decimal bagCost, Guid commodityGuid)
        {
            DataTable dt = _iu.GetWHRDetailById(whrId);
            if (dt.Rows.Count > 0)
            {
                DataTable dtCommGrade = _iu.GetCommodityGradeDetailById(new Guid(dt.Rows[0]["CommodityGradeId"].ToString()));
                if (dtCommGrade.Rows.Count > 0)
                {
                    decimal priceCalledPerKG = Convert.ToDecimal(dtCommGrade.Rows[0]["PriceCalledPerKG"]);
                    int lotsize = Convert.ToInt32(dtCommGrade.Rows[0]["LotSize"]);
                    if (new Guid(_cs["CoffeeGuid"].ToString()) == commodityGuid)
                    {
                        return Math.Round(((price / priceCalledPerKG) * (soldQty * lotsize)) + bagCost, 2);
                    }
                    return Math.Round((price / priceCalledPerKG) * (soldQty * lotsize), 2);
                }
                return 0.0M;
            }
            return 0.0M;
        }
        //((this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid].LotSizeInBag
        //                * c.Quantity) *
        //                this._Initialize.Warehouses [this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].WarehouseGuid].LocationDifferential, 2);
        public decimal CalculateLocationDifferential()
        {
            return 0;
        }
        //CFunctions.CalculateAdjustedPrice(this._Initialize, Order, type, c.LocationDifferential, c);
        public decimal CalculateAdjustedPrice(decimal price)
        {
            return Math.Round(price - CalculateLocationDifferential(), 2);
        }
        //CalculateActualWeight(this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].Weight, this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].OriginalQuantity) * Order.SoldQuantity, 2)
        public decimal CalculateActualWeight(Guid whrId, decimal soldQuantity)
        {
            DataTable dt = _iu.GetWHRDetailById(whrId);
            if (dt.Rows.Count > 0)
            {
                decimal netWeight = Convert.ToDecimal(dt.Rows[0]["NetWeight"]);
                decimal originalQuantity = Convert.ToDecimal(dt.Rows[0]["OriginalQuantity"]);
                return Math.Round((soldQuantity * netWeight) / originalQuantity, 2);
            }
            return 0.0M;
        }
        //AdjustedTradeValue = CFunctions.Round(((c.AdjustedPrice / Convert.ToDecimal(this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid].PriceCalledPerKG))
        //                * (((c.ActualWeight - c.MoistureLoss) * Convert.ToDecimal(this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid].DepositPerKG)))), 2);

        public decimal CalculateAdjustedTradeValue(decimal adjustedPrice, Guid whrId, decimal actualWeight, decimal moistureLoss)
        {
            DataTable dt = _iu.GetWHRDetailById(whrId);
            if (dt.Rows.Count > 0)
            {
                DataTable dtCommGrade = _iu.GetCommodityGradeDetailById(new Guid(dt.Rows[0]["CommodityGradeId"].ToString()));
                if (dtCommGrade.Rows.Count > 0)
                {
                    decimal priceCalledPerKG = Convert.ToDecimal(dtCommGrade.Rows[0]["PriceCalledPerKG"]);
                    decimal depositPerKG = Convert.ToDecimal(dtCommGrade.Rows[0]["DepositPerKG"]);
                    return Math.Round((adjustedPrice / priceCalledPerKG) * ((actualWeight - moistureLoss) * depositPerKG), 2);
                }
            }
            return 0.0M;
        }

        //CFunctions.Round(CFunctions.CalculateVAT(c.AdjustedTradeValue), 2);
        public decimal CalculateVAT(decimal adjustedTradeValue, Guid whrId, Guid ownerId, bool isMember)
        {
            bool vatExempt, vatable;
            string vatNumber;

            DataTable dt = _iu.GetWHRDetailById(whrId);
            if (dt.Rows.Count > 0)
            {
                DataTable dtCommGrade = _iu.GetCommodityGradeDetailById(new Guid(dt.Rows[0]["CommodityGradeId"].ToString()));
                if (dtCommGrade.Rows.Count > 0)
                {
                    if (dtCommGrade.Rows[0]["VATable"] == DBNull.Value)
                        return 0.0M;
                    else
                        vatable = Convert.ToBoolean(dtCommGrade.Rows[0]["VATable"]);
                }
                else
                {
                    return 0.0M;
                }
            }
            else
            {
                return 0.0M;
            }

            if (isMember)
            {
                var member = _iu.GetMemberDetailById(ownerId);
                if (member != null && member.Rows.Count > 0)
                {
                    vatExempt = member.Rows[0]["VatExempt"] != DBNull.Value ? Convert.ToBoolean(member.Rows[0]["VatExempt"]) : false;
                    vatNumber = member.Rows[0]["VATNo"] != DBNull.Value ? Convert.ToString(member.Rows[0]["VATNo"]) : null;
                    if (vatExempt == false && vatNumber != "" && vatable)
                    {
                        return Math.Round(adjustedTradeValue * 0.15M, 2);
                    }
                    else
                    {
                        return 0.0M;
                    }
                }
                return 0.0M;
            }
            else
            {
                var client = _iu.GetClientDetailById(ownerId);
                if (client != null && client.Rows.Count > 0)
                {
                    vatExempt = client.Rows[0]["VatExempt"] != DBNull.Value ? Convert.ToBoolean(client.Rows[0]["VatExempt"]) : false;
                    vatNumber = client.Rows[0]["VATNo"] != DBNull.Value ? Convert.ToString(client.Rows[0]["VATNo"]) : null;
                    if (vatExempt == false && vatNumber != "" && vatable)
                    {
                        return Math.Round(adjustedTradeValue * 0.15M, 2);
                    }
                    else
                    {
                        return 0.0M;
                    }
                }
                return 0.0M;
            }
            //if (ClientExist == true &&
            //   ObjClient.VATNO != "" &&
            //   ObjClient.VATExempt == false &&
            //   this._Initialize.WarehouseReceipts.ContainsKey(Order.WarehouseReceipt) == true &&
            //   this._Initialize.CommodityGradeDetails.ContainsKey(this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid) == true &&
            //   this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid].VATable == true)
            //{
            //    c.VAT = CFunctions.Round(CFunctions.CalculateVAT(c.AdjustedTradeValue), 2);
            //    c.TOT = 0;
            //}
            //else
            //{
            //    c.VAT = 0;
            //    if (ClientExist == true &&
            //        ObjClient.TOTExempt == false &&
            //        this._Initialize.WarehouseReceipts.ContainsKey(Order.WarehouseReceipt) == true &&
            //    this._Initialize.CommodityGradeDetails.ContainsKey(this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid) == true &&
            //    this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid].VATable == true)
            //    {
            //        c.TOT = CFunctions.Round(CFunctions.CalculateTOT(c.AdjustedTradeValue), 2);
            //    }
            //    else
            //    {
            //        c.TOT = 0;
            //    }
            //}
        }
        //c.DeductVATONNOR = this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid].DeductVATONNOR;

        //   c.TOT = CFunctions.Round(CFunctions.CalculateTOT(c.AdjustedTradeValue), 2);
        public decimal CalculateTOT(decimal adjustedTradeValue, Guid whrId, Guid ownerId, bool isMember)
        {
            bool totExempt, vatable;

            DataTable dt = _iu.GetWHRDetailById(whrId);
            if (dt.Rows.Count > 0)
            {
                DataTable dtCommGrade = _iu.GetCommodityGradeDetailById(new Guid(dt.Rows[0]["CommodityGradeId"].ToString()));
                if (dtCommGrade.Rows.Count > 0)
                {
                    if (dtCommGrade.Rows[0]["VATable"] == DBNull.Value)
                        return 0.0M;
                    else
                        vatable = Convert.ToBoolean(dtCommGrade.Rows[0]["VATable"]);
                }
                else
                {
                    return 0.0M;
                }
            }
            else
            {
                return 0.0M;
            }

            if (isMember)
            {
                var member = _iu.GetMemberDetailById(ownerId);
                if (member != null && member.Rows.Count > 0)
                {
                    totExempt = member.Rows[0]["TOTExempt"] != DBNull.Value ? Convert.ToBoolean(member.Rows[0]["TOTExempt"]) : false;
                    if (totExempt == false && vatable)
                    {
                        return Math.Round(adjustedTradeValue * 0.02M, 2);
                    }
                    else
                    {
                        return 0.0M;
                    }
                }
                return 0.0M;
            }
            else
            {
                var client = _iu.GetClientDetailById(ownerId);
                if (client != null && client.Rows.Count > 0)
                {
                    totExempt = client.Rows[0]["TOTExepmt"] != DBNull.Value ? Convert.ToBoolean(client.Rows[0]["TOTExepmt"]) : false;
                    if (totExempt == false && vatable)
                    {
                        return Math.Round(adjustedTradeValue * 0.02M, 2);
                    }
                    else
                    {
                        return 0.0M;
                    }
                }
                return 0.0M;
            }
            //if (this._Initialize.Member.ContainsKey(Order.SellMemberGuid) == true &&
            //    this._Initialize.Member[Order.SellMemberGuid].VATNO != "" &&
            //    this._Initialize.Member[Order.SellMemberGuid].VATExempt == false &&
            //    this._Initialize.WarehouseReceipts.ContainsKey(Order.WarehouseReceipt) == true &&
            //    this._Initialize.CommodityGradeDetails.ContainsKey(this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid) == true &&
            //    this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid].VATable == true)
            //{
            //    c.VAT = CFunctions.Round(CFunctions.CalculateVAT(c.AdjustedTradeValue), 2);
            //    c.TOT = 0;
            //}
            //else
            //{
            //    c.VAT = 0;
            //    if (this._Initialize.Member.ContainsKey(Order.SellMemberGuid) == true &&
            //        this._Initialize.Member[Order.SellMemberGuid].TOTExempt == false &&
            //        this._Initialize.WarehouseReceipts.ContainsKey(Order.WarehouseReceipt) == true &&
            //    this._Initialize.CommodityGradeDetails.ContainsKey(this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid) == true &&
            //    this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid].VATable == true)
            //    {
            //        c.TOT = CFunctions.Round(CFunctions.CalculateTOT(c.AdjustedTradeValue), 2);
            //    }
            //    else
            //    {
            //        c.TOT = 0;
            //    }

        }

        public decimal CalculateWT(decimal adjustedTradeValue, Guid ownerId, bool isMember)
        {
            //adjustedTradeValue*0.02
            bool wtExempt;
            if (isMember)
            {
                var member = _iu.MemberGetDetailsForWT(ownerId.ToString());
                if (member != null && member.Rows.Count > 0)
                {

                    wtExempt = member.Rows[0]["WithHoldingExepmt"] != DBNull.Value ? Convert.ToBoolean(member.Rows[0]["WithHoldingExepmt"]) : false;
                    if (wtExempt == false)
                    {
                        return Math.Round(adjustedTradeValue * 0.02M, 2);
                    }
                    else
                    {
                        return 0.0M;
                    }
                }
                return 0.0M;
            }
            else
            {
                var client = _iu.GetClientDetailById(ownerId);
                if (client != null && client.Rows.Count > 0)
                {
                    wtExempt = client.Rows[0]["WithHoldingExepmt"] != DBNull.Value ? Convert.ToBoolean(client.Rows[0]["WithHoldingExepmt"]) : false;
                    if (wtExempt == false)
                    {
                        return Math.Round(adjustedTradeValue * 0.02M, 2);
                    }
                    else
                    {
                        return 0.0M;
                    }
                }
                return 0.0M;
            }
        }

        //c.TradeFee = CFunctions.Round(CFunctions.CalculateTradeFee(((Order.Price / Convert.ToDecimal(this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid].PriceCalledPerKG))
        //                * (Order.SoldQuantity* this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid].LotSize))), 2);
        //c.TradeFee = c.TradeFee + CFunctions.Round(((c.BagCost* Convert.ToDecimal(0.4)) / 100),2);

        public decimal CalculateTradeFee(decimal price, Guid whrId, decimal soldQty, decimal bagCost)
        {
            DataTable dt = _iu.GetWHRDetailById(whrId);
            if (dt.Rows.Count > 0)
            {
                DataTable dtCommGrade = _iu.GetCommodityGradeDetailById(new Guid(dt.Rows[0]["CommodityGradeId"].ToString()));
                if (dtCommGrade.Rows.Count > 0)
                {
                    decimal priceCalledPerKG = Convert.ToDecimal(dtCommGrade.Rows[0]["PriceCalledPerKG"]);
                    int lotsize = Convert.ToInt32(dtCommGrade.Rows[0]["LotSize"]);
                    return Math.Round(Convert.ToDecimal(_cs["TradeFee"]) * (((price / priceCalledPerKG) * (soldQty * lotsize)) + ((bagCost * Convert.ToDecimal(0.4)) / 100)), 2);
                }
                return 0.0M;
            }
            return 0.0M;
        }
        public decimal CalculateBagCost(int noOfBags)
        {
            return Math.Round(Convert.ToDecimal(_cs["BagCost"]) * noOfBags, 2);
        }
        //c.HandlingCharge = CFunctions.Round((c.Quantity* this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid].LotSizeInBag) *
        //                this._Initialize.Warehouses [this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].WarehouseGuid].SellHandlingCharge, 2);

        public decimal CalculateHandlingCharge(decimal soldQty, Guid whrId, bool isSell)
        {
            DataTable dt = _iu.GetWHRDetailById(whrId);
            if (dt.Rows.Count > 0)
            {
                Guid cFTID = dt.Rows[0]["CFTID"] == DBNull.Value ? Guid.Empty : new Guid(dt.Rows[0]["CFTID"].ToString());
                if (cFTID != Guid.Empty)
                {//Trade is CFT determine option 
                    DataTable dtOptionDetail = _ic.GetContractOptionDetailByCFTID(cFTID);
                    if (dtOptionDetail.Rows.Count > 0)
                    {
                        if (!Convert.ToBoolean(dtOptionDetail.Rows[0]["HasHandling"])) return 0.0M;
                    }
                    else
                    {
                        return 0.0M;
                    }
                }

                int lotSizeInBag;

                DataTable dtCommGrade = _iu.GetCommodityGradeDetailById(new Guid(dt.Rows[0]["CommodityGradeId"].ToString()));
                DataTable dtWarehouse = _iu.GetWarehouseDetailById(new Guid(dt.Rows[0]["WarehouseId"].ToString()));
                if (dtCommGrade.Rows.Count > 0)
                {
                    lotSizeInBag = Convert.ToInt32(dtCommGrade.Rows[0]["LotsizeInBag"]);
                }
                else
                {
                    return 0.0M;
                }
                if (dtWarehouse.Rows.Count > 0)
                {
                    if (isSell)
                    {
                        return Math.Round(soldQty * lotSizeInBag * Convert.ToDecimal(dtWarehouse.Rows[0]["SellHandlingCharge"]), 2);
                    }
                    else
                    {
                        return Math.Round(soldQty * lotSizeInBag * Convert.ToDecimal(dtWarehouse.Rows[0]["BuyHandlingCharge"]), 2);
                    }
                }
                return 0.0M;
            }
            return 0.0M;
        }
        //c.StorageCharge = CFunctions.CalculateStorage(this._Initialize, c, this._Initialize.WarehouseReceipts[Order.WarehouseReceipt],
        //                this._Initialize.Warehouses[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].WarehouseGuid], this._Initialize.CommodityGradeDetails[this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].CommodityGradeGuid]);

        public decimal CalculateStorageFee(decimal soldQTY, Guid whrId, DateTime trdaeDate)
        {
            DataTable dt = _iu.GetWHRDetailById(whrId);
            if (dt.Rows.Count > 0)
            {
                bool isTraceable = Convert.ToBoolean(dt.Rows[0]["IsTraceable"]);
                Guid cFTID = dt.Rows[0]["CFTID"] == DBNull.Value ? Guid.Empty : new Guid(dt.Rows[0]["CFTID"].ToString());
                if (cFTID != Guid.Empty)
                {//Trade is CFT determine option 
                    DataTable dtOptionDetail = _ic.GetContractOptionDetailByCFTID(cFTID);
                    if (dtOptionDetail.Rows.Count > 0)
                    {
                        if (!Convert.ToBoolean(dtOptionDetail.Rows[0]["HasStorage"])) return 0.0M;
                    }
                    else
                    {
                        return 0.0M;
                    }
                }
                decimal StorageCharge = 0;
                DateTime tDate = trdaeDate.Date;
                DateTime dDate = Convert.ToDateTime(dt.Rows[0]["DateDeposited"]).Date;
                DataTable dtCommodity = _iov.CFTGetCommodityByGrade(new Guid(dt.Rows[0]["CommodityGradeId"].ToString()));
                Guid CommodityGuid = new Guid(dtCommodity.Rows[0]["CommodityGuid"].ToString());
                Guid CommodityClassGuid = new Guid(dtCommodity.Rows[0]["CommodityClassGuid"].ToString());

                DataTable dtCommGrade = _iu.GetCommodityGradeDetailById(new Guid(dt.Rows[0]["CommodityGradeId"].ToString()));
                DataTable dtWarehouse = _iu.GetWarehouseDetailById(new Guid(dt.Rows[0]["WarehouseId"].ToString()));

                int lotSizeInBag = Convert.ToInt32(dtCommGrade.Rows[0]["LotsizeInBag"]);

                int diff = (tDate.Date.Subtract(dDate.Date).Days);

                if (CommodityGuid == new Guid("D97FD8C1-8916-4E3D-89E2-BD50D556A32F") || CommodityGuid == new Guid("6af8e122-023a-4157-821e-a7c717ebeaa5") || CommodityGuid == new Guid("e55687a3-9bce-4a91-8359-aa63c81725e5"))
                {//Sesame OR Niger Seed OR Pigeon Peas
                    if (diff > 30)
                    {
                        StorageCharge = 0;
                        decimal strorageBefor30Day = Math.Round((soldQTY * lotSizeInBag) *
                            Convert.ToDecimal(dtWarehouse.Rows[0]["StorageCharge"]) * 30, 2);
                        decimal strorageAfter30Day = Math.Round((soldQTY * lotSizeInBag) *
                            (Convert.ToDecimal(dtWarehouse.Rows[0]["StorageCharge"]) * 2) * ((diff - 30)), 2);

                        StorageCharge = strorageBefor30Day + strorageAfter30Day;
                    }
                    else
                    {
                        StorageCharge = Math.Round((soldQTY * lotSizeInBag) * Convert.ToDecimal(dtWarehouse.Rows[0]["StorageCharge"]) * (diff), 2);
                    }
                }
                else if (CommodityGuid == new Guid("0BA2E68D-AEFD-4C17-986F-526A0F267DDE"))
                {//Haricot Beans
                    if (CommodityClassGuid != new Guid("f0591211-b4f0-4b28-b763-9550787e52ab"))
                    {
                        if (diff > 30)
                        {
                            StorageCharge = 0;
                            decimal strorageBefor30Day = Math.Round((soldQTY * lotSizeInBag) *
                                  Convert.ToDecimal(dtWarehouse.Rows[0]["StorageCharge"]) * 30, 2);
                            decimal strorageAfter30Day = Math.Round((soldQTY * lotSizeInBag) *
                                (Convert.ToDecimal(dtWarehouse.Rows[0]["StorageCharge"]) * 2) * ((diff - 30)), 2);

                            StorageCharge = strorageBefor30Day + strorageAfter30Day;
                        }
                        else
                        {
                            StorageCharge = Math.Round((soldQTY * lotSizeInBag) *
                               Convert.ToDecimal(dtWarehouse.Rows[0]["StorageCharge"]) * (diff), 2);
                        }
                    }
                    else
                    {
                        StorageCharge = Math.Round((soldQTY * lotSizeInBag) *
                                Convert.ToDecimal(dtWarehouse.Rows[0]["StorageCharge"]) * (diff), 2);
                    }
                }
                else if (CommodityGuid == new Guid("37f6bca2-bee9-4d93-bef6-3f60bf696f1d")
                    || CommodityGuid == new Guid("33ec4235-f37f-4e12-a393-282b70b8b441")
                    || CommodityGuid == new Guid("68686672-5a84-45a5-8736-a2c281d234cb")
                    || CommodityGuid == new Guid("99071B48-2D3D-4A2F-BBAD-2747E773CCB3")
                    || CommodityGuid == new Guid("37D28859-5579-436B-98C8-2BF28BD413BE"))
                {//Green Mung Bean || Soya Beans || Chick Pea || Maize || Wheat  
                    StorageCharge = Math.Round((soldQTY * lotSizeInBag) * Convert.ToDecimal(dtWarehouse.Rows[0]["StorageCharge"]) * (diff), 2);
                }
                else if (CommodityGuid == new Guid("71604275-df23-4449-9dae-36501b14cc3b"))
                {//Coffee
                    if (!isTraceable)
                    {//non-traceable coffee/old coffee
                        if (diff > 3)
                        {
                            StorageCharge = Math.Round((soldQTY * lotSizeInBag) * Convert.ToDecimal(dtWarehouse.Rows[0]["StorageCharge"]) * (diff), 2);
                        }
                        else
                        {
                            StorageCharge = 0;
                        }
                    }
                    else
                    {//Traceable coffee/new coffee
                        if (diff > 1)
                        {
                            StorageCharge = Math.Round((soldQTY * lotSizeInBag) * Convert.ToDecimal(dtWarehouse.Rows[0]["StorageCharge"]) * (diff), 2);
                        }
                        else
                        {
                            StorageCharge = 0;
                        }
                    }
                }
                return StorageCharge;
            }
            return 0.0M;
        }
        //c.CoffeeSportClub = CFunctions.Round(CFunctions.CalculateCoffeeSportClub((CFunctions.CalculateActualWeight(this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].Weight, this._Initialize.WarehouseReceipts[Order.WarehouseReceipt].OriginalQuantity) * Order.SoldQuantity)), 2);
        public decimal CalculateCoffeeSportClub(decimal soldQty, Guid whrId, Guid ownerId, bool isMember)
        {
            decimal actualWeight = CalculateActualWeight(whrId, soldQty);
            DataTable dt = _iu.GetWHRDetailById(whrId);
            Guid commodityGuid = new Guid(_iov.CFTGetCommodityByGrade(new Guid(dt.Rows[0]["CommodityGradeId"].ToString())).Rows[0]["CommodityGuid"].ToString());
            if (commodityGuid == new Guid(_cs["CoffeeGuid"]))
            {
                if (dt.Rows.Count > 0)
                {
                    if (isMember)
                    {
                        var member = _iu.MemberGetDetailsForWT(ownerId.ToString());
                        if (!Convert.ToBoolean(member.Rows[0]["CoffeeSportClub"]))
                        {
                            Math.Round(actualWeight * 0.03M, 2);
                        }
                        return 0.0M;
                    }
                    else
                    {
                        var client = _iu.GetClientDetailById(ownerId);
                        if (!Convert.ToBoolean(client.Rows[0]["NoClubContribution"]))
                        {
                            Math.Round(actualWeight * 0.03M, 2);
                        }
                        return 0.0M;
                    }
                }
            }
            return 0.0M;
        }

        public decimal CalculateRebaggingFee(Guid whrId, decimal soldQty)
        {
            DataTable dt = _iu.GetWHRDetailById(whrId);
            string grnNOList = dt.Rows[0]["GRNNumber"].ToString();
            DataTable dtGRns = _iu.GetGRNService(grnNOList);
            decimal rebaggingFee = 0.0M;
            decimal grnQty = Convert.ToDecimal(_iu.GetGRNDetail(grnNOList).Rows[0]["Quantity"]);
            DataTable dtCommGrade = _iu.GetCommodityGradeDetailById(new Guid(dt.Rows[0]["CommodityGradeId"].ToString()));
            if (dtGRns.Rows.Count > 0)
            {
                foreach (DataRow dr in dtGRns.Rows)
                {
                    decimal serviceFee = 0.0M;
                    if (dr["ServiceId"].ToString() == "2a0cb964-98f9-4627-b559-bef8e3471953")
                    {
                        serviceFee = Convert.ToDecimal(_cs["2a0cb964-98f9-4627-b559-bef8e3471953"]);
                    }
                    else if (dr["ServiceId"].ToString() == "0987052e-4d14-4986-88ea-81b7d9a02536")
                    {
                        serviceFee = Convert.ToDecimal(_cs["0987052e-4d14-4986-88ea-81b7d9a02536"]);
                    }
                    else if (dr["ServiceId"].ToString() == "4b7b3ba0-a341-4868-8fe8-097a15f5c938")
                    {
                        serviceFee = Convert.ToDecimal(_cs["4b7b3ba0-a341-4868-8fe8-097a15f5c938"]);
                    }
                    else if (dr["ServiceId"].ToString() == "afe055b2-9a73-42ad-9452-8c00b2df2eaa")
                    {
                        serviceFee = Convert.ToDecimal(_cs["afe055b2-9a73-42ad-9452-8c00b2df2eaa"]);
                    }
                    rebaggingFee += ((grnQty * serviceFee) / (Convert.ToDecimal(dt.Rows[0]["OriginalQuantity"]) * Convert.ToInt32(dtCommGrade.Rows[0]["LotsizeInBag"]))) *
                        (Convert.ToInt32(dtCommGrade.Rows[0]["LotsizeInBag"]) * soldQty);
                }
            }
            return rebaggingFee;
        }

        public decimal CalculateIFFee()
        {
            return 0;
        }

        public decimal CalculateClearingFee()
        {
            return 0;
        }

        public decimal CalculateOtherCharge(Guid whrId, decimal soldQty)
        {
            return CalculateIFFee() + CalculateClearingFee() + CalculateRebaggingFee(whrId, soldQty);
        }

        public decimal CalculateECXVAT(Guid whrId, decimal price, decimal soldQty, DateTime tradeDate, bool isSell, int noOfBags)
        {
            return CalculateRebaggingFee(whrId, soldQty) + CalculateClearingFee() + CalculateIFFee() +
                CalculateStorageFee(soldQty, whrId, tradeDate) + CalculateHandlingCharge(soldQty, whrId, isSell) +
                CalculateTradeFee(price, whrId, soldQty, CalculateBagCost(noOfBags)) + CalculateOtherCharge(whrId, soldQty);
        }

        public decimal CalculateTotalNetObligation(bool isSellOrder, decimal adjustedTradeValue, decimal vat, decimal tot, decimal wt)
        {
            if (isSellOrder)
            {
                return Math.Round(adjustedTradeValue + vat + tot - wt, 2);
            }
            else
            {
                return Math.Round(adjustedTradeValue + vat + tot, 2);
            }
        }

    }
}
