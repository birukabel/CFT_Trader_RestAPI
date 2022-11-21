using CFTTraderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Services
{
    public interface IOrderValidator
    {
        public string ValidateContractOrder(Contract contract,int orderType);
        public string ValidateEditOrder(Guid orderId, decimal price, Guid updatedBy);
    }
}
