using CFTTraderAPI.DataAccess;
using CFTTraderAPI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Repository
{
    public class UtilitiesRepository: IUtilities
    {
        private readonly IDataAccessProvider _db;
        private IConfiguration _cs;
        // private readonly IOptions<MyConfiguration> config;
        public UtilitiesRepository(IDataAccessProvider db, IConfiguration cs)
        {
            _cs = cs;
            _db = db;
        }

        //Get All Commodity Grades
        public DataTable GetCommodityGradeDetailById(Guid commodityGradeId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@CommodityGradeId");
           
            paramValue.Add(commodityGradeId);
        
            return _db.ExecuteDataTable(_cs["ConnectionStrings:lookupConnection"], "dbo", "spCFTGetCommodityGradeDetailById", paramName, paramValue, ref erroMesg);

        }

        //Get WHR details
        public DataTable GetWHRDetailById(Guid wHRId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@Id");

            paramValue.Add(wHRId);

            return _db.ExecuteDataTable(_cs["ConnectionStrings:cdConnection"], "dbo", "spCFTGetWHRDetailById", paramName, paramValue, ref erroMesg);
        }

        public DataTable GetClientDetailById(Guid clientId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@ClientId");

            paramValue.Add(clientId);

            return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "spCFTGetClientDetailById", paramName, paramValue, ref erroMesg);
        }

        public DataTable GetMemberDetailById(Guid memberId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@MemberId");

            paramValue.Add(memberId);

            return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "spCFTGetMemberDetailById", paramName, paramValue, ref erroMesg);
        }

        public DataTable MemberGetDetailsForWT(string memberId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@MEMBERIDLIST");

            paramValue.Add(memberId);

            return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "tblMemberGetDetails", paramName, paramValue, ref erroMesg);
        }

        public DataTable GetWarehouseDetailById(Guid warehouseId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@WarehouseId");

            paramValue.Add(warehouseId);

            return _db.ExecuteDataTable(_cs["ConnectionStrings:lookupConnection"], "dbo", "spCFTGetWarehouseDetailById", paramName, paramValue, ref erroMesg);
        }

        public DataTable GetGRNService(string gRNNOLIST)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@GRNNOLIST");

            paramValue.Add(gRNNOLIST);

            return _db.ExecuteDataTable(_cs["ConnectionStrings:warehouseConnection"], "dbo", "spGetGRNService", paramName, paramValue, ref erroMesg);
        }

        
        public DataTable GetGRNDetail(string gRN_Number)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@GRN_Number");

            paramValue.Add(gRN_Number);

            return _db.ExecuteDataTable(_cs["ConnectionStrings:warehouseConnectionCurrentDB"], "dbo", "spCFTGetGRNDetail", paramName, paramValue, ref erroMesg);
        }

        public DataTable GetTodaysSession()
        {
            var erroMesg = "";
            return _db.ExecuteDataTable(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spCFTGetTodaysSession", ref erroMesg);
        }

        public DataTable GetRepresentativeDetailByRepId(Guid repId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@RepId");

            paramValue.Add(repId);

            return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "spCFTGetRepresentativeDetailByRepId", paramName, paramValue, ref erroMesg);            
        }

        public DataTable GetSessionByCommodityGrade(Guid commodityGrade)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@CommodityGrade");

            paramValue.Add(commodityGrade);

            return _db.ExecuteDataTable(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spCFTGetSessionByCommodityGrade", paramName, paramValue, ref erroMesg);
        }
        public DataTable GetSessionCommodityPricelimit(Guid sessionId, Guid commodityGrade, Guid warehouseId, int productionYear)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
          
            paramName.Add("@sessionId");
            paramName.Add("@commodityGrade");
            paramName.Add("@warehouseId");
            paramName.Add("@productionYear");


            paramValue.Add(sessionId);
            paramValue.Add(commodityGrade);
            paramValue.Add(warehouseId);
            paramValue.Add(productionYear);

            return _db.ExecuteDataTable(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spGetCFTSessionCommodityPricelimit", paramName, paramValue, ref erroMesg);
        }

        public DataTable GetPriceLimitByContract(string symbol,Guid warhouseId,int productionYear)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            
            paramName.Add("@Symbol");
            paramName.Add("@WarhouseId");
            paramName.Add("@ProductionYear");

            paramValue.Add(symbol);
            paramValue.Add(warhouseId);
            paramValue.Add(productionYear);
            
            return _db.ExecuteDataTable(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spCFTGetPriceLimitByContract", paramName, paramValue, ref erroMesg);

        }

        public DataTable GetWHRByCFTID(Guid cFTID, Guid commodityGradeId, int transactionType)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            
            paramName.Add("@CFTID");
            paramName.Add("@CommodityGradeId");
            paramName.Add("@TransactionType");

            paramValue.Add(cFTID);
            paramValue.Add(commodityGradeId);
            paramValue.Add(transactionType);

            return _db.ExecuteDataTable(_cs["ConnectionStrings:cdConnection"], "dbo", "spCFTGetWHRByCFTID", paramName, paramValue, ref erroMesg);

        }

        
    }
}
