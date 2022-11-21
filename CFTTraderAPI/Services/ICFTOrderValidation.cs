using CFTTraderAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Services
{
    public interface ICFTOrderValidation
    {
        public DataTable GetTraderLicenseDetail(Guid commodityGuid, Guid ownerId, bool isMember);

        public DataTable GetTraderCOCDetail(Guid commodityGuid, Guid ownerId, bool isMember);

        public DataTable CFTGetClientMCADetail(Guid clientId, Guid memberId, Guid commodityId);

        public DataTable CFTGetCommodityByGrade(Guid commodityGrade);
    }
}
