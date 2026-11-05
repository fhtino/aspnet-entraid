using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace aspnetframework48
{
    public partial class main : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.User?.Identity?.IsAuthenticated == true)
            {
                UserInfoLabel.Text = $"IsAuthenticated: {HttpContext.Current.User?.Identity.IsAuthenticated} | " +
                                     $"Name: {HttpContext.Current.User?.Identity.Name} | " +
                                     $"AuthenticationType: {HttpContext.Current.User?.Identity.AuthenticationType}";
                LogoutButton.Visible = true;
            }
        }


        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            // Sign out the user.
            // Important: do not call Authentication.SignOut() without specifying the authentication type, otherwise it will sign out even from Entra ID
            Context.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            Response.Redirect("~/");
        }
    }
}