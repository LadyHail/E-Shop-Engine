﻿using System.Web;
using System.Web.Mvc;
using E_Shop_Engine.Services.Data.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace E_Shop_Engine.Website.Extensions
{
    public static class IdentityHelpers
    {
        public static MvcHtmlString GetUserName(this HtmlHelper html, string id)
        {
            AppUserManager manager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            return new MvcHtmlString(manager.FindByIdAsync(id).Result.UserName);
        }
    }
}