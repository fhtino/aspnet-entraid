using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace aspnetframework48.user_area
{

    public partial class _default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            ClaimsIdentity claimsIdentity = Context.User.Identity as ClaimsIdentity;

            if (claimsIdentity != null)
            {
                DataGrid1.DataSource = claimsIdentity.Claims.Select(c => new { c.Type, c.Value }).ToList();
                DataGrid1.DataBind();
            }
        }

    }

}