//$Header:$
//
//  Global.asax.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the global file.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using LST;
using log4net;
using System.Reflection;
using System.Web.SessionState;
using System.IO;

namespace LST
{
    public class Global : HttpApplication
    {
        //protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected static readonly ILog Log = LogManager.GetLogger(typeof(Global));

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            //AuthConfig.RegisterOpenAuth();
            //RouteConfig.RegisterRoutes(RouteTable.Routes);

            log4net.Config.XmlConfigurator.Configure();
            Log.Info("Application Started");

        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        

        void Application_Error(object sender, EventArgs e)
        {
            Exception _ex = Server.GetLastError();

            string url = HttpContext.Current.Request.Url.AbsolutePath.ToString();
            string msg = _ex.Message;
            msg += @"   Inner Exception: " + _ex.InnerException;

            const string lineSearch = ":line ";
            var index = _ex.StackTrace.LastIndexOf(lineSearch);
            string lineNumberText = string.Empty;
            if (index != -1)
            {
                lineNumberText = _ex.StackTrace.Substring(index + lineSearch.Length);
            }
            else
            {
                lineNumberText = "none found";
            }



            //Log.Error(url, _ex);
            //Response.Write("url: " +  url + "   " + "Error: " + _ex.Message.ToString());

            // Clear the error from the server
            Server.ClearError();

            if (_ex is HttpRequestValidationException)
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.Write(@"<html><head><title>HTML Not Allowed</title> </head>
                            <body style='font-family: Arial, Sans-serif;'><h1>Error Page!</h1>
                            <p>Error: for security reason, some characters are not allowed.</p>
                            <p>Please make sure that your inputs do not contain any angle brackets like &lt; or &gt;.</p>
                            <p><a href='javascript:history.go(-1);'>Go back</a></p></body></html>");
                Response.End();
            }
            else
            {
                Response.Clear();
                Response.Write(@"<html><head><title>Error Page</title> </head>
                            <body style='font-family: Arial, Sans-serif;'><h1>Error Page!</h1>
                            <p>We're sorry. Something unexpected happened. <br /><br /></p>" +
                            @"<p>" + msg + @"<br /><br /></p>" +
                            @"<p> line number: " + lineNumberText + @"<br /><br /></p>" +
                            @"<p>" + url + @"<br /><br /></p>" +
                            @"<p> Please <a href=https://slacspace.slac.stanford.edu/Operations/SCCS/AppDev/request>Contact AppDev team</a> for help.  Thank you!</p></body></html>");
                Response.End();
            }



        }

        void Session_Start(object sender, EventArgs e)
        {
            string protocol = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"];
            if (protocol == null || protocol == "0")
                protocol = "http://";
            else
                protocol = "https://";

            Session["urlhost"] = protocol + Request.Url.Host;
        }
    }

}
