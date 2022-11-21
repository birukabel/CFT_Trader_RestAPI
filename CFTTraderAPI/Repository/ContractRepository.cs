using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CFTTraderAPI.DataAccess;
using CFTTraderAPI.Models;
using CFTTraderAPI.Services;
using Microsoft.Extensions.Configuration;

namespace CFTTraderAPI.Repository
{
    public class ContractRepository : IContract
    {
        private readonly IDataAccessProvider _db;
        private IConfiguration _cs;
        // private readonly IOptions<MyConfiguration> config;
        public ContractRepository(IDataAccessProvider db, IConfiguration cs)
        {
            _cs = cs;
            _db = db;
        }

        //Get All Contracts for the logged in user

        public DataTable GetActiveContractByMember(Guid MemberId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("MemberID");
            paramValue.Add(MemberId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "spCFTGetActiveContractByMember", paramName, paramValue, ref erroMesg);
        }

        //Get cash balance
        public DataTable GetCashBalance(Guid MemberId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("MemberId");
            paramValue.Add(MemberId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:cdConnection"], "dbo", "spCFTGetCashBalance", paramName, paramValue, ref erroMesg);

        }

        //Post method to save order data to database
        public bool SaveOrder(Contract contract)
        {
            var erroMesg = "";

            ArrayList paramName = new ArrayList();

            paramName.Add("CFTID");
            paramName.Add("TransactionType");
            paramName.Add("Quantity");
            paramName.Add("Price");
            paramName.Add("CommodityGrade");
            paramName.Add("RepId");
            paramName.Add("SessionId");
            paramName.Add("RTC");
            paramName.Add("WHRId");

            ArrayList paramVal = new ArrayList();

            paramVal.Add(contract.CFTID);
            paramVal.Add(contract.TransactionType);
            paramVal.Add(contract.Quantity);
            paramVal.Add(contract.Price);
            paramVal.Add(contract.CommodityGrade);
            paramVal.Add(contract.RepId);
            paramVal.Add(contract.SessionId);
            paramVal.Add(contract.RTC);
            paramVal.Add(contract.WHRId);
            return _db.ExecuteNonQuery(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spCFTSaveOrder", paramName, paramVal, ref erroMesg);
        }

        bool SaveOrderHistory(Guid orderID, decimal oldPrice, decimal newPrice, Guid createdBy)
        {
            var erroMesg = "";

            ArrayList paramName = new ArrayList();

            paramName.Add("OrderID");
            paramName.Add("OldPrice");
            paramName.Add("NewPrice");
            paramName.Add("CreatedBy");

            ArrayList paramVal = new ArrayList();

            paramVal.Add(orderID);
            paramVal.Add(oldPrice);
            paramVal.Add(newPrice);
            paramVal.Add(createdBy);

            return _db.ExecuteNonQuery(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spCFTSaveOrderHistory", paramName, paramVal, ref erroMesg);
        }

        public bool EditOrder(Guid ID, decimal price, Guid updatedBy)
        {
            var erroMesg = "";

            ArrayList paramName = new ArrayList();

            paramName.Add("ID");
            paramName.Add("Price");
            paramName.Add("UpdatedBy");

            ArrayList paramVal = new ArrayList();

            paramVal.Add(ID);
            paramVal.Add(price);
            paramVal.Add(updatedBy);

            return _db.ExecuteNonQuery(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spCFTEditOrder", paramName, paramVal, ref erroMesg);

        }

        public bool CancelOrder(Guid id, Guid createdBy)
        {
            var erroMesg = "";

            ArrayList paramName = new ArrayList();

            paramName.Add("ID");
            paramName.Add("CreatedBy");
            ArrayList paramVal = new ArrayList();
            paramVal.Add(id);
            paramVal.Add(createdBy);
            return _db.ExecuteNonQuery(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spCFTCancelOrder", paramName, paramVal, ref erroMesg);
        }

        public Contract GetContractOrderById(Guid orderId)
        {
            Contract order = new Contract();
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("Id");
            paramValue.Add(orderId);
            DataTable data= _db.ExecuteDataTable(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spCFTGetContractOrder", paramName, paramValue, ref erroMesg);
            if (data.Rows.Count > 0)
            {
                order.ID = orderId;
                order.CreatedBy= new Guid(data.Rows[0]["CreatedBy"].ToString());
                order.OrderStatus= Convert.ToInt16(data.Rows[0]["OrderStatus"]); ;
                order.RTC=data.Rows[0]["RTC"].ToString();
                order.CFTID = new Guid(data.Rows[0]["CFTID"].ToString());
                order.CommodityGrade = new Guid(data.Rows[0]["CommodityGrade"].ToString());
                order.Quantity = Convert.ToDecimal(data.Rows[0]["Quantity"]);
                order.RepId = new Guid(data.Rows[0]["RepId"].ToString());
                order.SessionId = new Guid(data.Rows[0]["SessionId"].ToString());
                order.TicketNo = data.Rows[0]["TicketNo"].ToString();
                order.TransactionType = Convert.ToInt16(data.Rows[0]["TransactionType"]);
                order.WHRId = new Guid(data.Rows[0]["WHRId"].ToString());


            }
            return order;
        }
        public DataTable GetContractDetailById(Guid ID)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("ID");
            paramValue.Add(ID);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "spCFTGetContractDetailById", paramName, paramValue, ref erroMesg);

        }
        public DataTable GetTraderByTraderID(Guid TraderID)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@TraderID");
            paramValue.Add(TraderID);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "spCFTGetTraderByTraderID", paramName, paramValue, ref erroMesg);

        }
        public DataTable GetOrderView(Guid repID)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("RepID");
            paramValue.Add(repID);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spCFTGetOrderView", paramName, paramValue, ref erroMesg);
        }
        public SqlCommand CreateSaveOrder(Contract contract)
        {
            SqlCommand cmd = new SqlCommand();

            SqlConnection con = new SqlConnection(_cs["ConnectionStrings:tradeConnectionString"]);
            cmd.CommandText = "dbo.spCFTSaveOrder";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CFTID", contract.CFTID);
            cmd.Parameters.AddWithValue("@TransactionType", contract.TransactionType);
            cmd.Parameters.AddWithValue("@Quantity", contract.Quantity);
            cmd.Parameters.AddWithValue("@Price", contract.Price);
            cmd.Parameters.AddWithValue("@CommodityGrade", contract.CommodityGrade);
            cmd.Parameters.AddWithValue("@RepId", contract.RepId);
            cmd.Parameters.AddWithValue("@SessionId", contract.SessionId);
            cmd.Parameters.AddWithValue("@RTC", contract.RTC);
            cmd.Parameters.AddWithValue("@WHRId", contract.WHRId);

            cmd.Connection = con;
            return cmd;
        }
        public SqlCommand CreateEditOrder(Contract contract)
        {
            SqlCommand cmd = new SqlCommand();

            SqlConnection con = new SqlConnection(_cs["ConnectionStrings:tradeConnectionString"]);
            cmd.CommandText = "dbo.spCFTEditOrder";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", contract.ID);
            cmd.Parameters.AddWithValue("@Price", contract.Price);
            cmd.Parameters.AddWithValue("@UpdatedBy", contract.RepId);

            cmd.Connection = con;
            return cmd;
        }
        public SqlCommand CreateBuySellTicket(Guid cftId, Guid whrNo, Guid buyTicketId, Guid sellTicketId)
        {

            SqlCommand cmd = new SqlCommand();
            SqlConnection con = new SqlConnection(_cs["ConnectionStrings:tradeConnectionString"]);
            cmd.CommandText = "dbo.spExecuteCFTtBuySellTickets";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CFTId", cftId);
            cmd.Parameters.AddWithValue("@WHRId", whrNo);
            cmd.Parameters.AddWithValue("@BuyTicketId", buyTicketId);
            cmd.Parameters.AddWithValue("@SellTicketId", sellTicketId);

            cmd.Connection = con;
            return cmd;
        }
        public SqlCommand CreateTrade(Guid buyTicketId, Guid sellTicketId)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection con = new SqlConnection(_cs["ConnectionStrings:tradeConnectionString"]);

            cmd.CommandText = "dbo.spCFTExecuteTrades";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BuyTicketId", buyTicketId);
            cmd.Parameters.AddWithValue("@SellTicketId", sellTicketId);
            cmd.Connection = con;
            return cmd;
        }
        public SqlCommand CreateOrderStatus(Guid whrId, int status)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection con = new SqlConnection(_cs["ConnectionStrings:tradeConnectionString"]);
            cmd.CommandText = "dbo.spCFTUpdateOrderStatus";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@WHRId", whrId);
            cmd.Parameters.AddWithValue("@OrderStatus", status);
            cmd.Connection = con;
            return cmd;
        }
        public SqlCommand CreateRemainingQuanity(Guid cftId, decimal quantity)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection con = new SqlConnection(_cs["ConnectionStrings:tradeConnectionString"]);
            cmd.CommandText = "dbo.spCFTUpdateRemainingQuantity";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CFTId", cftId);
            cmd.Parameters.AddWithValue("@TradedQuantity", quantity);
            cmd.Connection = con;
            return cmd;
        }
        public DataTable GetContractOrderByWHRID(Guid wHRId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("WHRId");
            paramValue.Add(wHRId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spCFTGetContractOrderByWHRID", paramName, paramValue, ref erroMesg);
        }

        public bool RejectTrade(Guid sellerTicketID, Guid buyTicketId, Guid wHRId)
        {
            var erroMesg = "";

            ArrayList paramName = new ArrayList();

            paramName.Add("SellerTicketID");
            paramName.Add("BuyTicketId");
            paramName.Add("WHRId");

            ArrayList paramVal = new ArrayList();

            paramVal.Add(sellerTicketID);
            paramVal.Add(buyTicketId);
            paramVal.Add(wHRId);

            return _db.ExecuteNonQuery(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spCFTRejectTrade", paramName, paramVal, ref erroMesg);
        }
        public DataTable GetExecutedTrades(Guid ownerId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("OwnerId");
            paramValue.Add(ownerId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spCFTGetExecutedTrades", paramName, paramValue, ref erroMesg);
        }

        public DataTable GetContractOptionDetail(int iD)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("ID");
            paramValue.Add(iD);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "spCFTGetContractOptionDetail", paramName, paramValue, ref erroMesg);

        }

        public string ExecuteTransaction(SqlCommand saveOrderCommand, SqlCommand buySellTicketCommand, SqlCommand tradeCommand, SqlCommand updateOrderStatus, SqlCommand updateRemainingQty)
        {
            return _db.ExecuteSqlTransaction(saveOrderCommand, buySellTicketCommand, tradeCommand, updateOrderStatus, updateRemainingQty);
        }

        public bool ExecuteCFTTrade(string sellerTicketNo, string buyerTicketNo, Guid buyTicketId, Guid sellTicketId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateOrderStatus(Guid wHRId, int orderStatus)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRemainingWeight(Guid cftId, decimal quantity)
        {
            throw new NotImplementedException();
        }

        public DataTable GetContractOptionDetailByCFTID (Guid cFTID)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("CFTID");
            paramValue.Add(cFTID);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "spCFTGetContractOptionDetailByCFTID", paramName, paramValue, ref erroMesg);
        }

        
    }
}
