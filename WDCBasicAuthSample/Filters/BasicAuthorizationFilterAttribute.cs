using System;
using System.DirectoryServices.AccountManagement;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Configuration;

namespace WDCBasicAuthSample.Filters
{
    public class BasicAuthenticationIdentity : GenericIdentity
    {
        public BasicAuthenticationIdentity(string name, string password)
            : base(name, "Basic")
        {
            this.Password = password;
        }

        /// <summary>
        /// Basic Auth Password for custom authentication
        /// </summary>
        public string Password { get; set; }
    }

    public class BasicAuthorizationFilterAttribute : System.Web.Http.Filters.AuthorizationFilterAttribute
    {
        private Func<UserPrincipal, string> accessChecker;

        public BasicAuthorizationFilterAttribute()
        {
            // Set a default checked which doesn't check anything else about the user
            this.accessChecker = u => null;
        }

        public BasicAuthorizationFilterAttribute(Func<UserPrincipal, string> accessChecker)
        {
            this.accessChecker = accessChecker;
        }
        
        private void SetUnauthorizedResponse(HttpActionContext actionContext, string msg)
        {
            actionContext.Response = actionContext.Request.CreateResponse<string>(HttpStatusCode.Unauthorized, msg);
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var identity = ParseAuthorizationHeader(actionContext);
            if (identity == null)
            {
                SetUnauthorizedResponse(actionContext, "Missing username or password");
                return;
            }

            string domain = ConfigurationManager.AppSettings["Domain"].ToString();
            using (var context = new PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Domain, domain))
            {
                if (!context.ValidateCredentials(identity.Name, identity.Password))
                {
                    SetUnauthorizedResponse(actionContext, "Invalid Credentials");
                    return;
                }

                var searcher = new PrincipalSearcher(new UserPrincipal(context) { SamAccountName = identity.Name });
                var userResult = searcher.FindOne() as UserPrincipal;
                if (userResult == null)
                {
                    SetUnauthorizedResponse(actionContext, "Unable to find user '" + identity.Name + "'");
                    return;
                }

                var checkResult = this.accessChecker(userResult);
                if (!string.IsNullOrEmpty(checkResult))
                {
                    SetUnauthorizedResponse(actionContext, checkResult);
                    return;
                }
            }
        }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.Run(new Action(() => OnAuthorization(actionContext)));
        }

        protected BasicAuthenticationIdentity ParseAuthorizationHeader(HttpActionContext actionContext)
        {
            string authHeader = null;
            var auth = actionContext.Request.Headers.Authorization;
            if (auth != null && auth.Scheme == "Basic")
                authHeader = auth.Parameter;

            if (string.IsNullOrEmpty(authHeader))
                return null;

            authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

            var tokens = authHeader.Split(':');
            if (tokens.Length < 2)
                return null;

            // Grab the second half of the header which is the password
            var password = authHeader.Substring(tokens[0].Length + 1);
            return new BasicAuthenticationIdentity(tokens[0], password);
        }
    }
}