using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace WDCBasicAuthSample.Filters
{
    public class GroupMembershipAuthorizationFilterAttribute : BasicAuthorizationFilterAttribute
    {
        public GroupMembershipAuthorizationFilterAttribute() : base(CheckGroupMembership)
        {

        }

        public static string CheckGroupMembership(UserPrincipal user)
        {
            var strGroups = ConfigurationManager.AppSettings["AllowedGroups"].ToString();
            var allowedGroups = strGroups.Split(',').ToList();

            var matchingGroup = user.GetAuthorizationGroups().FirstOrDefault(p => allowedGroups.Contains(p.SamAccountName));
            
            if (matchingGroup == null)
            {
                return "User is not a member of a required group";
            }

            // We've passed the test! return null to indicate everything is ok
            return null;
        }
    }
}