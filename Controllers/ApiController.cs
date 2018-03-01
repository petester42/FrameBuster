using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace FrameBuster.Controllers
{
    [Route("html")]
    public class ApiController : Controller
    {
        [HttpGet]
        public ContentResult GetHtml([FromQuery]string url)
        {
            using (var client = new HttpClient())
            {
                var uri = new Uri(url);

                var request = new HttpRequestMessage()
                {
                    RequestUri = uri,
                    Method = HttpMethod.Get,
                };

                var chrome = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36";

                request.Headers.UserAgent.ParseAdd(chrome);
                // request.Headers.Add("Origin", "*");
                // request.Headers.Add("X-Requested-With", "*");

                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    string domain;
                    if (uri.Host.StartsWith("www."))
                    {
                        domain = $"{uri.Scheme}://{uri.Host}";
                    }
                    else
                    {
                        domain = $"{uri.Scheme}://www.{uri.Host}";
                    }

                    // read content and try to fix relative urls
                    var content = response.Content.ReadAsStringAsync().Result
                        .Replace("=\"/", $"=\"{domain}/")
                        .Replace("=\'/", $"=\'{domain}/")
                        .Replace(", /", $", {domain}/")
                        .Replace("url(/", $"url({domain}/");

                    return new ContentResult
                    {
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.OK,
                        Content = content
                    };
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
