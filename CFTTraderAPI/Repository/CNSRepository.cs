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
    public class CNSRepository:ICNS
    {
        private readonly IDataAccessProvider _db;
        private IConfiguration _cs;

        public CNSRepository(IDataAccessProvider db, IConfiguration cs)
        {
            _db = db;
            _cs = cs;
        }

        public DataTable CFTGetBuyExecutedTradesByOwner(Guid ownerId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@ownerId");

            paramValue.Add(ownerId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:tradeConnectionString"], "dbo", "spCFTGetBuyExecutedTradesByOwner", paramName, paramValue, ref erroMesg);
        }

        public DataTable GetBankAcountBalance(Guid ownerId)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();
            paramName.Add("@OwnerId");
            paramValue.Add(ownerId);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:cdConnection"], "dbo", "spCFTGetPayInAccountByOwner", paramName, paramValue, ref erroMesg);
            
        }
    }
}
