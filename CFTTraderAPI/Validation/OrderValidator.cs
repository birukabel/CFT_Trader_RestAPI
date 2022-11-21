using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CFTTraderAPI.Models;
using CFTTraderAPI.Services;
using CFTTraderAPI.Repository;
using Microsoft.Extensions.Configuration;

namespace CFTTraderAPI.Validation
{
    public class OrderValidator : IOrderValidator
    {
        //Licenese and COC (coffee) check
        //MCA check for client
        //Check maturity date
        //Check order qty <= contract qty
        //Balance check

        readonly ICNS _icns;
        readonly IConfiguration _cs;

        readonly IContract _ic;
        readonly ICFTOrderValidation _icftv;//to access DB methods
        readonly IClearingFunctions _icf;
        readonly IUtilities _iu;
        public OrderValidator(ICFTOrderValidation icftv, IContract ic, IConfiguration cs, ICNS icns, IClearingFunctions icf, IUtilities iu)
        {
            _icftv = icftv;
            _ic = ic;
            _cs = cs;
            _icns = icns;
            _icf = icf;
            _iu = iu;
        }

        //Validate Membership business licnense, MCA etc..
        string ValidateMembershipAttributes(Contract contract)
        {
            DataTable dt = _ic.GetContractDetailById(contract.CFTID);
            if (dt.Rows.Count >= 1)
            {
                Guid commodityId = new Guid(dt.Rows[0]["CommodityId"].ToString());
                Guid ownerId, memberId, clientId;
                ownerId = memberId = clientId = Guid.Empty;

                bool isMember = false;
                if (contract.TransactionType == 1 || contract.TransactionType == 2)//Buy or Sell
                {
                    //Get Member and ClientID
                    DataTable dt2 = _ic.GetTraderByTraderID(new Guid(dt.Rows[0]["CFTBuyId"].ToString()));
                    if (dt2.Rows.Count > 0)
                    {
                        memberId = new Guid(dt2.Rows[0]["MemberId"].ToString());
                        clientId = new Guid(dt2.Rows[0]["ClientId"].ToString());
                        if (memberId == clientId)//owner is member
                        {
                            ownerId = memberId;
                            isMember = true;
                        }
                        else//owner is client 
                        {
                            ownerId = clientId;
                        }
                        if (_icftv.GetTraderLicenseDetail(commodityId, ownerId, isMember).Rows.Count > 0)
                        {
                            if (commodityId == new Guid(_cs["CoffeeGuid"]))
                            {
                                if (_icftv.GetTraderCOCDetail(commodityId, ownerId, isMember).Rows.Count > 0)
                                {
                                    if (isMember)
                                    {//No MCA Check
                                        return "Valid";
                                    }
                                    else
                                    {//Check MCA for client
                                        if (_icftv.CFTGetClientMCADetail(clientId, memberId, commodityId).Rows.Count > 0)
                                        {
                                            return "Valid";
                                        }
                                        else
                                        {
                                            return @"Client doesn't have active MCA with member for the commodity";
                                        }
                                    }
                                }
                                else
                                {
                                    return @"Contract owner doesn't have valid COC";
                                }
                            }
                            else
                            {
                                return "Valid";
                            }
                        }
                        else
                        {
                            return @"Contract owner doesn't have valid business license";
                        }
                    }
                    else
                    {
                        return "No data is registered for contract traders";
                    }
                }
                else
                {
                    return "Order is neither sell or buy";
                }
            }
            else
            {
                return "No contract exists for the order";
            }
            return "Not Valid";
        }

        //Vlidate balance realted issues
        string ValidateCNSAttributes(Contract contract)
        {
            DataTable dt = _ic.GetContractDetailById(contract.CFTID);
            if (dt.Rows.Count >= 1)
            {
                bool isMember = false;
                DataTable dt2 = _ic.GetTraderByTraderID(new Guid(dt.Rows[0]["CFTBuyId"].ToString()));
                Guid ownerId, memberId, clientId;
                ownerId = memberId = clientId = Guid.Empty;
                if (dt2.Rows.Count > 0)
                {
                    memberId = new Guid(dt2.Rows[0]["MemberId"].ToString());
                    clientId = new Guid(dt2.Rows[0]["ClientId"].ToString());
                    if (memberId == clientId)//owner is member
                    {
                        ownerId = memberId;
                        isMember = true;
                    }
                    else//owner is client 
                    {
                        ownerId = clientId;
                    }
                    DataTable _bankAccount = _icns.GetBankAcountBalance(ownerId);
                    if (_bankAccount.Rows.Count > 0)
                    {
                        DataTable dtTrades = _ic.GetExecutedTrades(ownerId);
                        //call method to calculate clearing
                        decimal totalNetObligation = TradeClearing(dtTrades, contract, ownerId, isMember);
                        //call method to retrive balance

                        decimal balance = Convert.ToDecimal(_bankAccount.Rows[0]["Balance"] == DBNull.Value ? 0.0M : _bankAccount.Rows[0]["Balance"]);
                        if (balance < totalNetObligation)
                        {
                            return @"Buyer member doesn't have enough balance";
                        }
                        else
                        {
                            return "Valid";
                        }
                    }
                    else
                    {
                        return "In Valid";
                    }
                }
            }
            else
            {
                return "In Valid";
            }
            return "In Valid";
        }

        decimal TradeClearing(DataTable dtTrades, Contract contract, Guid ownerId, bool isMember)
        {
            decimal totalNetObligations = 0;
            if (dtTrades.Rows.Count > 0)
            {
                //DataTable dtContract = _ic.GetContractDetailById(contract.CFTID);
                //DataTable dtContractTrader = _ic.GetTraderByTraderID(new Guid(dtContract.Rows[0]["CFTBuyId"].ToString()));
                decimal acceptedadjustedTradeValue = 0.0M, acceptedvat = 0.0M, acceptedadjustedPrice = 0.0M, acceptedactualweight = 0.0M, acceptedtot = 0.0M, acceptedwt = 0.0M;

                foreach (DataRow dr in dtTrades.Rows)
                {
                    acceptedadjustedPrice = Math.Round(_icf.CalculateAdjustedPrice(Convert.ToDecimal(dr["Price"])), 2);
                    acceptedactualweight = Math.Round(_icf.CalculateActualWeight(new Guid(dr["WRId"].ToString()), Convert.ToDecimal(dr["Quantity"])), 2);
                    acceptedadjustedTradeValue = _icf.CalculateAdjustedTradeValue(acceptedadjustedPrice, new Guid(dr["WRId"].ToString()), acceptedactualweight, 0.0M);
                    acceptedvat = _icf.CalculateVAT(acceptedadjustedTradeValue, new Guid(dr["WRId"].ToString()), ownerId, isMember);
                    acceptedtot = _icf.CalculateTOT(acceptedadjustedTradeValue, new Guid(dr["WRId"].ToString()), ownerId, isMember);
                    acceptedwt = _icf.CalculateTOT(acceptedadjustedTradeValue, new Guid(dr["WRId"].ToString()), ownerId, isMember);
                    totalNetObligations += _icf.CalculateTotalNetObligation(false, acceptedadjustedTradeValue, acceptedvat, acceptedtot, acceptedwt);
                }
            }
            decimal adjustedTradeValue = 0.0M, vat = 0.0M, adjustedPrice = 0.0M, actualweight = 0.0M, tot = 0.0M, wt = 0.0M;
            adjustedPrice = Math.Round(_icf.CalculateAdjustedPrice(Convert.ToDecimal(contract.Price)), 2);
            actualweight = Math.Round(_icf.CalculateActualWeight(contract.WHRId, contract.Quantity), 2);
            adjustedTradeValue = _icf.CalculateAdjustedTradeValue(adjustedPrice, contract.WHRId, actualweight, 0.0M);
            vat = _icf.CalculateVAT(adjustedTradeValue, contract.WHRId, ownerId, isMember);
            tot = _icf.CalculateTOT(adjustedTradeValue, contract.WHRId, ownerId, isMember);
            wt = _icf.CalculateTOT(adjustedTradeValue, contract.WHRId, ownerId, isMember);
            totalNetObligations += _icf.CalculateTotalNetObligation(false, adjustedTradeValue, vat, tot, wt);

            return totalNetObligations;
        }

        decimal GetTraderCashBalance(Contract contract, Guid ownerId)
        {
            DataTable dtRep = _iu.GetRepresentativeDetailByRepId(contract.RepId);
            if (dtRep.Rows.Count > 0)
            {
                DataTable dtCashBalance = _ic.GetCashBalance(new Guid(dtRep.Rows[0]["MemberId"].ToString()));
                decimal balance = 0.0M;
                if (dtCashBalance.Rows.Count > 0)
                {
                    balance = dtCashBalance.AsEnumerable().Where(x => (new Guid(x["OwnerGuid"].ToString())) == ownerId).Sum(y => Convert.ToDecimal(y["Balance"]));
                    return balance;
                }
                return 0.0M;
            }
            return 0.0M;
        }

        //Checks for contract related  attributes like qty, matrurity date etc.
        string ValidateContractAttributes(Contract contract)
        {
            //check maturity date
            //check order qty <= contract qty
            DataTable dt = _ic.GetContractDetailById(contract.CFTID);
            if (dt.Rows.Count >= 1)
            {
                Guid commodityId = new Guid(dt.Rows[0]["CommodityId"].ToString());
                if (new Guid(_icftv.CFTGetCommodityByGrade(contract.CommodityGrade).Rows[0]["CommodityGuid"].ToString()) == commodityId)
                {
                    if (((DateTime.Now.Date - Convert.ToDateTime(dt.Rows[0]["ContractDate"]).Date)).Days >= Convert.ToInt16(_cs["contractDays"]))
                    {//Contract Date is valid

                        if (Convert.ToDateTime(dt.Rows[0]["MaturityDate"]) >= DateTime.Now
                            && (Convert.ToDateTime(dt.Rows[0]["MaturityDate"]).Date - Convert.ToDateTime(dt.Rows[0]["ContractDate"]).Date).Days <= Convert.ToInt16(_cs["maxMaturityDate"]))
                        {//Maturity date is later than today 
                            if (!new List<int>() { 3, 4, 7, 8 }.Contains(Convert.ToInt16(dt.Rows[0]["StatusId"])))
                            {//Valid Contract Status 
                                return "Contract status is not Approved";
                            }
                        }
                        else
                        {
                            return "Contract maturity date should be later than today";
                        }
                    }
                    else
                    {
                        return "The contract needs " + _cs["contractDays"] + " days prior to trading";
                    }
                }
                return "Valid";
            }
            return "Not Valid";
        }

        //calls each validation
        public string ValidateContractOrder(Contract contract,int orderType)//1=new , 2=edit
        {
            DataTable dtCFTSession = _iu.GetTodaysSession();
            if (dtCFTSession.Rows.Count > 0 && Convert.ToInt32(dtCFTSession.Rows[0]["Status"]) == 2)
            {
                if (contract is null)
                {
                    return "contract entity is null";
                }
                else
                {
                    string strResult = ValidateContractSaveEntity(contract);
                    if (strResult == "")
                    {
                        strResult = ValidateMembershipAttributes(contract);
                        if (strResult == "Valid")
                        {
                            strResult = ValidateContractAttributes(contract);
                            if (strResult == "Valid")
                            {

                                //CNS Validation
                                if (contract.TransactionType == 1)
                                {

                                    strResult = ValidateCNSAttributes(contract);

                                }
                                if (strResult == "Valid")
                                {
                                    //Save contract order
                                    DataTable dtSession = _iu.GetSessionByCommodityGrade(contract.CommodityGrade);
                                    if (dtSession.Rows.Count > 0)
                                    {
                                        Guid? sessionId = new Guid(dtSession.Rows[0]["SessionId"].ToString());
                                        if (sessionId != null)
                                        {
                                            var commodityGrade = contract.CommodityGrade;
                                            DataTable cftContract = _ic.GetContractDetailById(contract.CFTID);
                                            if (dtCFTSession.Rows.Count > 0)
                                            {
                                                if (Math.Round(contract.Quantity, 2) <= Math.Round(Convert.ToDecimal(cftContract.Rows[0]["RemainingQuantity"]), 2))
                                                {
                                                    int py = Convert.ToInt32(cftContract.Rows[0]["ProductionYear"]);
                                                    Guid warehouseId = new Guid((cftContract.Rows[0]["ECXWarehouseId"]).ToString());
                                                    DataTable dtSessionCommodityPL = _iu.GetSessionCommodityPricelimit((Guid)sessionId, commodityGrade, warehouseId, py);
                                                    if (dtSessionCommodityPL.Rows.Count > 0)
                                                    {
                                                        bool hasPriceLimit = (bool)dtSessionCommodityPL.Rows[0]["HasPriceLimit"];
                                                        if (hasPriceLimit)
                                                        {
                                                            dynamic lowerLimit = dtSessionCommodityPL.Rows[0]["LowerPriceLimit"];
                                                            dynamic uppperLimit = dtSessionCommodityPL.Rows[0]["UpperPriceLimit"];

                                                            if (contract.Price >= lowerLimit && contract.Price <= uppperLimit)
                                                            {
                                                                DataTable dtCon = _ic.GetContractOrderByWHRID(contract.WHRId);
                                                                if ((dtCon.Rows.Count == 1 && orderType==1) || (dtCon.Rows.Count == 2 && orderType == 2))
                                                                {//Execute Trade with transaction
                                                                    string sellTicketNo = "", buyTicketNo = "";
                                                                    Guid sellTicketId = Guid.NewGuid(), buyTicketId = Guid.NewGuid();
                                                                    decimal price = 0.0M;
                                                                    foreach (DataRow dr in dtCon.Rows)
                                                                    {

                                                                        if (Convert.ToInt16(dr["TransactionType"]) == 1)
                                                                        {
                                                                            if (dr["TicketNo"].ToString() != contract.TicketNo)
                                                                            {
                                                                                price = Convert.ToDecimal(dr["Price"]);
                                                                                buyTicketNo = dr["TicketNo"].ToString();
                                                                            }
                                                                            else
                                                                            {
                                                                                continue;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (dr["TicketNo"].ToString() != contract.TicketNo)
                                                                            {
                                                                                price = Convert.ToDecimal(dr["Price"]);
                                                                                sellTicketNo = dr["TicketNo"].ToString();
                                                                            }
                                                                            else
                                                                            {
                                                                                continue;
                                                                            }
                                                                        }
                                                                    }
                                                                    //Call Method Execute
                                                                    return ExecuteTrade(contract, price, sellTicketId, buyTicketId, sellTicketNo, buyTicketNo,orderType);
                                                                }
                                                                else
                                                                {
                                                                    if (orderType == 1)
                                                                    {
                                                                        if (_ic.SaveOrder(contract))
                                                                        {
                                                                            return "OK";
                                                                        }
                                                                        else
                                                                        {
                                                                            return "Error while saving contract order";
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (_ic.EditOrder(contract.ID, contract.Price, contract.RepId))
                                                                        {
                                                                            return "OK";
                                                                        }
                                                                        else
                                                                        {
                                                                            return "Error while Editing contract order";
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                return "Price should be bettween range";
                                                            }
                                                        }
                                                        else if (!hasPriceLimit)
                                                        {
                                                            //save
                                                        }
                                                        else
                                                        {
                                                            return "No price limit found";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return "No price limit for the commodity";
                                                    }
                                                }
                                                else
                                                {
                                                    return "Order quantity exceeds remaining contract quantity";
                                                }
                                            }
                                            else
                                            {
                                                return "No valid contract";
                                            }
                                            //Price limit

                                        }
                                        else
                                        {
                                            return "unable to find approprite session";
                                        }
                                    }
                                }
                                else
                                {
                                    return strResult;
                                }
                            }
                            else
                            {
                                return strResult;
                            }
                        }
                        else
                        {
                            return strResult;
                        }
                    }
                    else
                    {
                        return "please provide all required data";
                    }
                }
            }
            else
            {
                return "session is not opened to submit order";
            }
            return "Not Valid";
        }

        string ExecuteTrade(Contract contract, decimal price, Guid sellTicketId, Guid buyTicketId, string sellTicketNo, string buyTicketNo,int orderType)
        {
            if (orderType == 1)
            {
                if (contract.Price == price)
                {
                    return _ic.ExecuteTransaction(_ic.CreateSaveOrder(contract), _ic.CreateBuySellTicket(contract.CFTID, contract.WHRId, buyTicketId, sellTicketId),
                        _ic.CreateTrade(buyTicketId, sellTicketId), _ic.CreateOrderStatus(contract.WHRId, 2), _ic.CreateRemainingQuanity(contract.CFTID, contract.Quantity));

                }
                else
                {
                    if (_ic.SaveOrder(contract))
                    {
                        return "OK";
                    }
                    else
                    {
                        return "Error While Saving Order";
                    }
                }
            }
            else
            {
                if (contract.Price == price)
                {
                    return _ic.ExecuteTransaction(_ic.CreateEditOrder(contract), _ic.CreateBuySellTicket(contract.CFTID, contract.WHRId, buyTicketId, sellTicketId),
                        _ic.CreateTrade(buyTicketId, sellTicketId), _ic.CreateOrderStatus(contract.WHRId, 2), _ic.CreateRemainingQuanity(contract.CFTID, contract.Quantity));

                }
                else
                {
                    if (_ic.EditOrder(contract.ID, contract.Price, contract.RepId))
                    {
                        return "OK";
                    }
                    else
                    {
                        return "Error While Editing Order";
                    }
                }
            }

        }

        string ValidateContractSaveEntity(Contract contract)
        {
            var strValidateMsg = "";

            if (contract.CFTID == Guid.Empty)
                strValidateMsg = strValidateMsg + "CFTID, ";
            else if (contract.TransactionType != 1 && contract.TransactionType != 2)
                strValidateMsg = strValidateMsg + "TransactionType, ";
            else if (contract.Quantity == 0.0M)
                strValidateMsg = strValidateMsg + "Quantity, ";
            else if (contract.Price == 0.0M)
                strValidateMsg = strValidateMsg + "Price, ";
            else if (contract.CommodityGrade == Guid.Empty)
                strValidateMsg = strValidateMsg + "CommodityGrade, ";
            else if (contract.RepId == Guid.Empty)
                strValidateMsg = strValidateMsg + "RepId, ";
            else if (contract.SessionId == Guid.Empty)
                strValidateMsg = strValidateMsg + "SessionId, ";
            //else if (contract.OrderStatus == 0)
            //    strValidateMsg = strValidateMsg + "OrderStatus, ";
            //else if (contract.SubmitedTime == new DateTime())
            //    strValidateMsg = strValidateMsg + "SubmitedTime, ";
            else if (contract.RTC == "")
                strValidateMsg = strValidateMsg + "RTC, ";
            else if (contract.WHRId == Guid.Empty)
                strValidateMsg = strValidateMsg + "WHRId, ";
            else if (contract.CreatedBy == Guid.Empty)
                strValidateMsg = strValidateMsg + "CreatedBy, ";
            if (strValidateMsg != "")
                strValidateMsg = strValidateMsg + " value is not provided";
            return strValidateMsg;
        }

        public string ValidateEditOrder(Guid orderId, decimal price,Guid updatedBy)
        {
            Contract order = _ic.GetContractOrderById(orderId);
            order.Price = price;
            order.RepId = updatedBy;
            return ValidateContractOrder(order, 2);//2=edit
        }

        
    }
}
