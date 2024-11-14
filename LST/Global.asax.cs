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
        protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            //AuthConfig.RegisterOpenAuth();
            //RouteConfig.RegisterRoutes(RouteTable.Routes);

            log4net.Config.XmlConfigurator.Configure();
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            string _redirecturl = "";
            Exception _ex = Server.GetLastError();


            string url = HttpContext.Current.Request.Url.AbsolutePath.ToString();
            Log.Error(url, _ex);
        
              
             
            if (_ex is HttpRequestValidationException)
            {
                Server.ClearError();
                if (Session["urlhost"] != null)
                {
                    _redirecturl = Session["urlhost"] + Request.ApplicationPath + "/" + "Error.aspx?msg=sc";
                }
                else
                {
                    _redirecturl = "~/Error.aspx";
                }
                Response.Clear();
                Response.Redirect(_redirecturl);
            }
            else
            {
                Server.ClearError();
                Response.Clear();
                Response.Redirect("~/Error.aspx");
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
