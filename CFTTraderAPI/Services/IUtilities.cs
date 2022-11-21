using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Services
{
    public interface IUtilities
    {
        public DataTable GetCommodityGradeDetailById(Guid commodityGradeId);
        public DataTable GetWHRDetailById(Guid wHRId);

        public DataTable GetClientDetailById(Guid clientId);

        public DataTable GetMemberDetailById(Guid memberId);

        public DataTable MemberGetDetailsForWT(string memberId);

        public DataTable GetWarehouseDetailById(Guid warehouseId);

        public DataTable GetGRNService(string gRNNOLIST);

        public DataTable GetGRNDetail(string gRN_Number);

        public DataTable GetTodaysSession();

        public DataTable GetRepresentativeDetailByRepId(Guid repId);

        public DataTable GetSessionByCommodityGrade(Guid commodityGrade);
        public DataTable GetSessionCommodityPricelimit(Guid sessionId, Guid commodityGrade, Guid warehouseId, int productionYear);

        public DataTable GetPriceLimitByContract(string symbol, Guid warhouseId, int productionYear);

        public DataTable GetWHRByCFTID(Guid cFTID, Guid commodityGradeId, int transactionType);
    }
}
