using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace aspnetframework48
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Context?.User?.Identity?.IsAuthenticated != true)
            //{
            //    Context.GetOwinContext().Authentication.Challenge(
            //        new AuthenticationProperties { RedirectUri = Request.RawUrl },
            //        OpenIdConnectAuthenticationDefaults.AuthenticationType);
            //
            //    Response.StatusCode = 401;
            //    Context.ApplicationInstance.CompleteRequest();
            //}            
        }
    }
}