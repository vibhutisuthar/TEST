using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections;

namespace MTP_JAPA
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "User", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "Root", // Route name
                "", // URL with no parameters
                new { controller = "User", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

        }

        protected void Session_Start()
        {
            Session["Country"] = "AU";
            string u;
            u = Request.Url.Authority;

            if (u.ToLower() == "mytravelplans.com.sg")
            {
                Session["Country"] = "SG";
            }
            else if (u.ToLower() == "mytravelplans.com.au")
            {
                Session["Country"] = "AU";
            }
            else if (u.ToLower() == "mytravelplans.co.in")
            {
                Session["Country"] = "IN";
            }
            else if (u.ToLower() == "mytravelplans.eu")
            {
                Session["Country"] = "EU";
            }
            else
            {
                Session["Country"] = "AU";
            }

            //if (!PageBase.GetDebug())
            //{
            //    if (Request.Url.Scheme.ToLower() != "https")
            //    {
            //        string MyURL = Request.Url.ToString().Substring(4);

            //        if (MyURL.IndexOf("://") == -1)
            //        {
            //            MyURL = "://" + Request.Url.ToString().Substring(4);
            //        }

            //        MyURL = MyURL.ToLower();

            //        if (MyURL.IndexOf("www.") != -1)
            //        {
            //            MyURL = MyURL.Replace("www.", "");
            //        }

            //        //   Response.Redirect("https" + MyURL);

            //        return;

            //    }

            //    string newURL = Request.Url.ToString().ToLower();

            //    if (newURL.IndexOf("www.") != -1)
            //    {
            //        newURL = newURL.Replace("www.", "");
            //        Response.Redirect(newURL);
            //    }
            //}

            //Application.Lock();

            //try
            //{
            //    Hashtable MyUsers;
            //    if (Application["Users"] == null)
            //    {
            //        MyUsers = new Hashtable();
            //    }
            //    else
            //    {
            //        MyUsers = (Hashtable)Application["Users"];
            //    }

            //    Visits V = new Visits();
            //    V.IP = Request.UserHostAddress;
            //    if (!MyUsers.ContainsKey(V.IP))
            //        MyUsers.Add(Session.SessionID, V);

            //    Application["Users"] = MyUsers;
            //}
            //catch
            //{
            //}

            //Application.UnLock();
        }
    }
}