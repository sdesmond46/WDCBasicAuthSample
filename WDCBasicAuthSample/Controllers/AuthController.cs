using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WDCBasicAuthSample.Filters;

namespace WDCBasicAuthSample.Controllers
{
    [GroupMembershipAuthorizationFilterAttribute]
    public class AuthController : ApiController
    {
        [HttpGet]
        public string TestAuth()
        {
            return "OK";
        }
    }
}
