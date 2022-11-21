using CFTTraderAPI.DataAccess;
using CFTTraderAPI.Models;
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
    public class CFTOrderValidationRepository : ICFTOrderValidation
    {
        private readonly IDataAccessProvider _db;
        private IConfiguration _cs;
        // private readonly IOptions<MyConfiguration> config;
        public CFTOrderValidationRepository(IDataAccessProvider db, IConfiguration cs)
        {
            _cs = cs;
            _db = db;
        }

        public DataTable GetTraderLicenseDetail(Guid commodityGuid, Guid ownerId, bool isMember)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@commodityGuid");
            paramName.Add("@ownerId");

            paramValue.Add(commodityGuid);
            paramValue.Add(ownerId);

            if (isMember)//Validate for memeber
            {
                return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "spCFTValidateMemberLicense", paramName, paramValue, ref erroMesg);
            }
            else//Validate for client 
            {
                return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "spCFTValidateClientLicense", paramName, paramValue, ref erroMesg);
            }
        }

        public DataTable GetTraderCOCDetail(Guid commodityGuid, Guid ownerId, bool isMember)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@commodityGuid");
            paramName.Add("@ownerId");

            paramValue.Add(commodityGuid);
            paramValue.Add(ownerId);

            if (isMember)//Validate for memeber
            {
                return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "spCFTValidateMemberCOC", paramName, paramValue, ref erroMesg);
            }
            else//Validate for client 
            {
                return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "spCFTValidateClientCOC", paramName, paramValue, ref erroMesg);
            }
        }

        public DataTable CFTGetClientMCADetail(Guid clientId, Guid memberId,Guid commodityId)
        {            
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@clientId");
            paramName.Add("@memberId");
            paramName.Add("@commodityId");

            paramValue.Add(clientId);
            paramValue.Add(memberId);
            paramValue.Add(commodityId);
            
            return _db.ExecuteDataTable(_cs["ConnectionStrings:membershipConnection"], "dbo", "spCFTGetClientMCADetail", paramName, paramValue, ref erroMesg);
        }

        public DataTable CFTGetCommodityByGrade(Guid commodityGrade)
        {
            var erroMesg = "";
            ArrayList paramName = new ArrayList();
            ArrayList paramValue = new ArrayList();

            paramName.Add("@commodityGrade");
          
            paramValue.Add(commodityGrade);
            return _db.ExecuteDataTable(_cs["ConnectionStrings:lookupConnection"], "dbo", "spCFTGetCommodityByGrade", paramName, paramValue, ref erroMesg);
        }

    }
}
