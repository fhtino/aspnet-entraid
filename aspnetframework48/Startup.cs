using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Configuration;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(aspnetframework48.Startup))]

namespace aspnetframework48
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }


        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(
                new CookieAuthenticationOptions()
                //{
                //    Provider = new CookieAuthenticationProvider
                //    {
                //        OnApplyRedirect = context =>
                //        {
                //            var user = context.OwinContext.Authentication.User;

                //            if (user?.Identity?.IsAuthenticated == true)
                //            {
                //                var deniedPage = context.Request.PathBase + "/NotAuthorized.aspx";
                //                context.Response.Redirect(deniedPage);
                //            }
                //            else
                //            {
                //                context.Response.Redirect(context.RedirectUri);
                //            }
                //        }                      
                //    }
                //}
                );

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = ConfigurationManager.AppSettings["entraid:ClientId"],
                    Authority = ConfigurationManager.AppSettings["entraid:Authority"],
                    RedirectUri = ConfigurationManager.AppSettings["entraid:RedirectUri"],
                    ResponseType = OpenIdConnectResponseType.CodeIdToken,
                    Scope = "openid profile email",
                    TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        NameClaimType = "preferred_username"  // this will fill the User.Identity.Name with the preferred_username claim
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        SecurityTokenValidated = context =>
                        {
                            // This is just an example of adding a custom role claim
                            // In real cases, you should query a database using context.AuthenticationTicket.Identity.Name or other claims
                            // to get the roles for the user and add them to the claims collection.
                            context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "MyCustomRole"));
                            context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "admins"));

                            return Task.FromResult(0);
                        },
                        AuthenticationFailed = context =>
                        {
                            context.HandleResponse();
                            context.Response.Redirect("/Error");
                            return Task.FromResult(0);
                        },
                        RedirectToIdentityProvider = context =>
                        {
                            // This redirects an already-authenticated user when the app tries to send them to Entra ID again.
                            // Obviously, unauthenticated users can still sign in normally. 
                            if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication &&
                                context.OwinContext.Authentication.User?.Identity?.IsAuthenticated == true)
                            {
                                context.HandleResponse();

                                var pathBase = context.OwinContext.Request.PathBase.Value ?? String.Empty;
                                context.OwinContext.Response.Redirect(pathBase + "/NotAuthorized.aspx");
                            }

                            return Task.FromResult(0);
                        },
                    }
                });

            app.UseStageMarker(PipelineStage.Authenticate);

            //app.UseMyRoles(); // this is another approach for adding custom roles to the current user claims but it is called on every request, so it may not be the best approach for performance reasons.
        }

    }


    /*
    public static class AddMyRoles
    {
        public static IAppBuilder UseMyRoles(this IAppBuilder app)
        {
            if (app == null) { throw new ArgumentNullException("app"); }

            app.Use((context, next) =>
            {
                if (context.Authentication.User != null && context.Authentication.User.Identity.IsAuthenticated)
                {
                    // Add custom roles to the user claims
                    var identity = context.Authentication.User.Identity as System.Security.Claims.ClaimsIdentity;

                    if (identity != null)
                    {
                        // Example: Add a custom role claim
                        identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "MyCustomRole"));
                    }
                }

                return next.Invoke();
            });

            return app;
        }
    }*/

}
