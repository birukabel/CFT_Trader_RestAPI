using CFTTraderAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Services
{
    public interface IContract
    {
        public DataTable GetActiveContractByMember(Guid MemberId);

        public DataTable GetCashBalance(Guid MemberId);

        public bool SaveOrder(Contract contract);

        public bool EditOrder(Guid ID, decimal price, Guid updatedBy);

        public DataTable GetOrderView(Guid repID);

        public bool CancelOrder(Guid id, Guid createdBy);

        public DataTable GetContractDetailById(Guid ID);

        public DataTable GetTraderByTraderID(Guid TraderID);

        public DataTable GetContractOrderByWHRID(Guid wHRId);

        public bool ExecuteCFTTrade(string sellerTicketNo, string buyerTicketNo, Guid buyTicketId, Guid sellTicketId);

        public bool UpdateOrderStatus(Guid wHRId, int orderStatus);

        public bool RejectTrade(Guid sellerTicketID, Guid buyTicketId, Guid wHRId);
        public bool UpdateRemainingWeight(Guid cftId, decimal quantity);
        public DataTable GetExecutedTrades(Guid ownerId);
        public string ExecuteTransaction(SqlCommand saveOrderCommand, SqlCommand buySellTicketCommand, SqlCommand tradeCommand, SqlCommand updateOrderStatus, SqlCommand updateRemainingQty);
        public SqlCommand CreateBuySellTicket(Guid cftId, Guid whrNo, Guid buyTicketId, Guid sellTicketId);
        public SqlCommand CreateTrade(Guid buyTicketId, Guid sellTicketId);
        public SqlCommand CreateOrderStatus(Guid whrId, int status);
        public SqlCommand CreateRemainingQuanity(Guid cftId, decimal quantity);
        public SqlCommand CreateSaveOrder(Contract contract);
        public SqlCommand CreateEditOrder(Contract contract);
        public DataTable GetContractOptionDetailByCFTID(Guid cFTID);
        public Contract GetContractOrderById(Guid orderId);
    }
}
