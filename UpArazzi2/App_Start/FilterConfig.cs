using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UpArazzi2.App_Start
{
    public class FilterConfig
    {
        public class CompressAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                GZipEncodePage();
            }


            public static bool IsGZipSupported()
            {
                string AcceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];
                if (!string.IsNullOrEmpty(AcceptEncoding) &&
                        (AcceptEncoding.Contains("gzip") || AcceptEncoding.Contains("deflate")))
                    return true;
                return false;
            }


            public static void GZipEncodePage()
            {
                HttpResponse Response = HttpContext.Current.Response;

                if (IsGZipSupported())
                {
                    string AcceptEncoding = HttpContext.Current.Request.Headers["Accept-Encoding"];

                    if (AcceptEncoding.Contains("gzip"))
                    {
                        Response.Filter = new System.IO.Compression.GZipStream(Response.Filter,
                                                    System.IO.Compression.CompressionMode.Compress);
                        Response.Headers.Remove("Content-Encoding");
                        Response.AppendHeader("Content-Encoding", "gzip");
                    }
                    else
                    {
                        Response.Filter = new System.IO.Compression.DeflateStream(Response.Filter,
                                                    System.IO.Compression.CompressionMode.Compress);
                        Response.Headers.Remove("Content-Encoding");
                        Response.AppendHeader("Content-Encoding", "deflate");
                    }


                }


                Response.AppendHeader("Vary", "Content-Encoding");
            }
        }
    }
}