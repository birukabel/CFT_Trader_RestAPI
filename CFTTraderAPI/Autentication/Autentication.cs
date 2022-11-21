using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices;
using CFTTraderAPI.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CFTTraderAPI.Services;

namespace CFTTraderAPI.Autentication
{
    public class Autentication:IAutentication
    {
        static User user;

        readonly IConfiguration _cs;

        public Autentication(IConfiguration cs)
        {
            _cs = cs;
        }

        public string AutenticateUser(string userName, string password)
        {
            if (AutenticateToAD(_cs["DirPath"], _cs["domain"], userName, password, _cs["ACDUser"], _cs["ACDPass"]))
            {
                return CreateToken();
            }
            return "";
        }

        bool AutenticateToAD(string dirPath, string _domain, string userName, string pwd, string _adAdminUser, string _adAdminPass)
        {
            string domain = _domain;//SettingReader.ADDomainNameForEmployees;
            string LDAP_Path = dirPath;//SettingReader.ADPathForEmployees
            //string container = "OU=Trade, DC=Trade, DC=ECX, DC=com";
            string adAdminUser = _adAdminUser;//System.Configuration.ConfigurationManager.AppSettings["ACDUser"];
            string adAdminPass = _adAdminPass;//System.Configuration.ConfigurationManager.AppSettings["ACDPass"];

            if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(LDAP_Path))
                return false;
            string domainAndUsername = domain + "\\" + userName;

            try
            {
                #region Authenticate using Directory Search
                //DirectoryEntry entry = new DirectoryEntry(LDAP_Path, userName, pwd, AuthenticationTypes.Secure | AuthenticationTypes.Sealing);
                using (DirectoryEntry entry = new(LDAP_Path, userName, pwd, AuthenticationTypes.Secure | AuthenticationTypes.Sealing | AuthenticationTypes.Signing))
                {
                    //Bind to the native AdsObject to force authentication.
                    object obj = entry.NativeObject;
                    DirectorySearcher search = new DirectorySearcher(entry);

                    search.Filter = "(sAMAccountName=" + userName + ")";
                    search.PropertiesToLoad.Add("CN");
                    SearchResultCollection results = search.FindAll();
                    if (results == null || results.Count == 0)
                    {//no AD record found
                        return false;
                    }
                    if (results.Count > 1)
                    {//multiple AD records were found
                        results.Dispose();
                        return false;
                    }
                    SearchResult result = results[0];//take the first AD Record

                    if (result != null)
                    {
                        System.DirectoryServices.DirectoryEntry userADEntry = result.GetDirectoryEntry();
                        user = new();
                        user.ID = userADEntry.Guid;
                        user.Name = userADEntry.Username;
                        //Session["LoggedUser"] = userADID;
                    }
                    else
                    {
                        return false;
                    }
                    entry.Close();
                    return true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                return false;//authentication fails - let the AD handle the # of trials

                //throw new Exception("Error authenticating user. \n" + ex.Message);
            }
        }

        string CreateToken()
        {
            List<Claim> claims = new()
            {
                new("name", user.Name),
                new("Id", user.ID.ToString())
            };            

            claims.Add(new("role", "Rep"));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cs["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _cs["Jwt:Issuer"],
                _cs["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);

            return new  JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
