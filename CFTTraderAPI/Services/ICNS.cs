using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Services
{
    public interface ICNS
    {
        public DataTable CFTGetBuyExecutedTradesByOwner(Guid ownerId);
        public DataTable GetBankAcountBalance(Guid ownerId);
    }
}
