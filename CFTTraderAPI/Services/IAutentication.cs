using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFTTraderAPI.Services
{
    public interface IAutentication
    {
        public string AutenticateUser(string userName, string password);
    }
}
