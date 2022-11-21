using CFTTraderAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAutentication _autentication;
        public AuthenticationController(IAutentication autentication)
        {
            _autentication = autentication;
        }
        [HttpGet("autenticate")]

        public ActionResult Login(string userName, string password)
        {
            var saron_jil_felefelex_anotherjil = _autentication.AutenticateUser(userName, password);
            return Ok(saron_jil_felefelex_anotherjil);
        }
    }
}
