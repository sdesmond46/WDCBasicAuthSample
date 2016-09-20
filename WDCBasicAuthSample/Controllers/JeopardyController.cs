using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using WDCBasicAuthSample.Filters;

namespace WDCBasicAuthSample.Controllers
{
    [GroupMembershipAuthorizationFilter]
    public class JeopardyController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Schema()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "protected_data", "schema.json");
            string yourJson = File.ReadAllText(filePath);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpGet]
        public HttpResponseMessage Data()
        {
            var results = new JArray();
            
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "protected_data", "jeopardy_questions.json");
            StreamContent content = new StreamContent(new FileStream(filePath, FileMode.Open));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = content;
            return response;
        }

    }
}
