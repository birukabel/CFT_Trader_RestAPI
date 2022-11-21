using CFTTraderAPI.Models;
using CFTTraderAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize (Roles = "Rep")]
    public class ContractController : Controller
    {
        private readonly IContract _db;
        //private readonly IValidation _iv;
        private readonly ICFTOrderValidation _icftv;//to call DB methods
        private readonly IOrderValidator _iov;//to call validation methods
        private readonly IUtilities _iu;

        public ContractController(IContract db, IOrderValidator iov, ICFTOrderValidation icftv,IUtilities iu)
        {
            _db = db;
            _iov = iov;
            _icftv = icftv;
            _iu = iu;
        }

        [Route("positionsummary")]
        [HttpGet]
        public IActionResult CFTGetActiveContractByMember()
        {
            Guid repId = new Guid("751a7d05-1c09-4512-8ecd-090a18b13352");//new(HttpContext.User.FindFirst("id").Value);
            DataTable dt = _iu.GetRepresentativeDetailByRepId(repId);
            if (dt.Rows.Count > 0)
            {
                Guid memberId = new(dt.Rows[0]["MemberId"].ToString());
                var data = _db.GetActiveContractByMember(memberId);
                return Ok(data);
            }
            return Ok();
        }

        [Route("cashbalance")]
        [HttpGet]
        public IActionResult CFTGetCashBalance()
        {
            Guid repId = new Guid("751a7d05-1c09-4512-8ecd-090a18b13352"); //new(HttpContext.User.FindFirst("id").Value);
            DataTable dt = _iu.GetRepresentativeDetailByRepId(repId);
            if (dt.Rows.Count > 0)
            {
                Guid memberId = new(dt.Rows[0]["MemberId"].ToString());
                var data = _db.GetCashBalance(memberId);
                return Ok(data);
            }
            return Ok();           
        }


        [HttpGet("pricelimit")]
        public IActionResult GetPriceLimitByContract(string symbol, Guid warehouseId, int productionYear)
        {
            var data = _iu.GetPriceLimitByContract(symbol, warehouseId, productionYear);
            return Ok(data);
        }

        [HttpGet("whrbycftid")]
        public IActionResult GetWHRByCFTID(Guid cftId, Guid commodityGradeId, int transactionType)
        {
            var data = _iu.GetWHRByCFTID(cftId, commodityGradeId, transactionType);
            return Ok(data);
        }

        [HttpPost("saveorder")]
        public IActionResult SaveOrder(Contract contract)
        {
            //Calls for ValidateContractOrder()
            string result = _iov.ValidateContractOrder(contract,1);//1=save
            return Ok(result);
        }

        [HttpPut("editorder")]
        public IActionResult EditOrder(Guid orderId, decimal price,Guid updatedBy)
        {
            string result= _iov.ValidateEditOrder(orderId, price, updatedBy);
            return Ok(result);
        }

        [HttpGet("orderview")]
        public IActionResult GetOrderView(Guid repId)
        {
            var data = _db.GetOrderView(repId);
            return Ok(data);
        }

        [HttpPut("cancelorder")]
        public IActionResult CancelOrder(Guid orderId,Guid createdBy)
        {
            var data = _db.CancelOrder(orderId, createdBy);
            if (!data)
                return BadRequest();
            return Ok(data);
        }


    }
}
