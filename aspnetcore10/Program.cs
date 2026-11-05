using aspnetcore10.code;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace aspnetcore10
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false);

            // Add environment variables to Configuration at the very end so they override everything.
            // This is required on Azure to be sure that the environment variables
            // are used instead of the appsettings.json values.
            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                            .AddMicrosoftIdentityWebApp(builder.Configuration, "AzureAd");
            //              .EnableTokenAcquisitionToCallDownstreamApi() // This is needed to exchange the authorization code for an ID Token
            //              .AddInMemoryTokenCaches();

            builder.Services.Configure<OpenIdConnectOptions>(
                OpenIdConnectDefaults.AuthenticationScheme,
                options =>
                {
                    options.Events.OnTokenValidated += async context =>
                    {
                        if (context.Principal?.Identity is not ClaimsIdentity identity)
                            return;

                        // This is just an example. For real cases, resolve roles from a trusted source,
                        // such as your application database, using the validated user's ID (for example, the "objectidentifier" claim and "tenantid").                        
                        // ClaimConstants.ObjectId
                        // ClaimConstants.TenantId

                        // see readme.md
                        string[] roles = new[] { "users", /*"admins"*/ };
                        foreach (var role in roles)
                            identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    };
                });

            //new MicrosoftIdentityOptions()
            //{

            //};


            builder.Services.ConfigureAll<CookieAuthenticationOptions>(options =>
                {
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.AccessDeniedPath = "/AccessDenied";
                });

            builder.Services.AddRazorPages(options =>
                {
                    options.Conventions.AuthorizeFolder("/user_area", "NormalUsersOnly");
                    options.Conventions.AuthorizeFolder("/admin_area", "AdminsOnly");
                });

            builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy("NormalUsersOnly", policy => policy.RequireRole("users"));
                    options.AddPolicy("AdminsOnly", policy => policy.RequireRole("admins"));
                });

            // ----------------------------------------------------------------------------

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            //app.UseMiddleware<MyRolesMiddleware>(app.Configuration);  // see readme.md
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
