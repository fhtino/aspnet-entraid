using System.Security.Claims;
using System.Security.Principal;

namespace aspnetcore10.code
{

    public class MyRolesMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;


        public MyRolesMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var ci = context.User.Identity as ClaimsIdentity;
                if (ci != null)
                {
                    // This is just an example. Roles should come from database.
                    string[] roles = new[] { "users", /*"admins"*/ };
                    foreach (var role in roles)
                        ci.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }

            await _next(context);
        }
    }

}
