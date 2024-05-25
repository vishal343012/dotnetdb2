using System;
using System.Data;
using System.Linq;
using Dapper;
using Jitbit.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHIndiaAdminPanel.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using Backend.Models;

namespace NHIndiaAdminPanel.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        // GET: Login
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult SignOut()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        [HttpGet]
        public MessageStatus Login(string UserName, string Password)
        {
            try
            {
                using (var cn = DBUtils.GetNewOpenConnection())
                {
                    string SqlString = "SPEmployeeLogin";
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserName", UserName);
                    parameters.Add("@Password", EncryptData.EncryptPassword(Password));

                    var result = cn.QueryFirstOrDefault<LoginResult>(SqlString, parameters, commandType: CommandType.StoredProcedure);

                    if (result != null && result.Status == 1)
                    {
                        HttpContext.Session.SetString("IsLoggedIn", "1");
                        HttpContext.Session.SetString("EmpCode", result.EMPEmpCode);
                        HttpContext.Session.SetString("EmployeeId", result.EmployeeId.ToString());
                        HttpContext.Session.SetString("EmpUserName", result.EmpUserName);
                        HttpContext.Session.SetString("AccessName", result.AccessName);

                        Response.Cookies.Append("ASP.NET_SessionId", Guid.NewGuid().ToString(), new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(50000)
                        });

                        return new MessageStatus
                        {
                            Status = result.Status,
                            Message = result.Message
                        };
                    }
                    else
                    {
                        return new MessageStatus
                        {
                            Status = result.Status,
                            Message = result.Message
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login.");
                return new MessageStatus
                {
                    Status = 0,
                    Message = "An error occurred during login."
                };
            }
        }
    }

    public class LoginResult
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string EMPEmpCode { get; set; }
        public int EmployeeId { get; set; }
        public string EmpUserName { get; set; }
        public string AccessName { get; set; }
    }



}
