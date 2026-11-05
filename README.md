# aspnet-entraid examples and tests

Basic examples of Asp.net Core and Asp.net Framework web applications with Azure Entra ID authentication.  
  
**!!!THIS CODE IS NOT PRODUCTION READY!!!**


## Entra ID : application setup on Azure

[...todo...]

- the application registration does not require the creation of a secret


## Aspnetcore10 project

### local dev setup

- create a appsettings.local.json (git ignored) with the following content:

```json
{
  "$schema": "https://raw.githubusercontent.com/AzureAD/microsoft-identity-web/refs/heads/master/JsonSchemas/microsoft-identity-web.json",
  
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "-tenant_id-",
    "ClientId": "-application_client_id-"
  }
}
```

### Roles

Custom roles should come from a database and can be injected with two different approaches:
 - #1 at the login : lightweight, done only one time but the roles remain fix for the life of the session cookie.
 - #2 on every http request : on every request, roles are added. More heavy but allow to change the roles of a user when they are already logged in. With this approach, a memory cache of a couple of minute is recommended to avoid stressing the database.

#### Approach #1

Roles are injected at login using the `OnTokenValidated` event in `OpenIdConnectOptions`:

```
  builder.Services.Configure<OpenIdConnectOptions>(
                OpenIdConnectDefaults.AuthenticationScheme,
                options =>
                {
                    options.Events.OnTokenValidated += async context => {...}
```

#### Approach #2

Roles are injected on every request. A custom middleware is used.
```
...
app.UseMiddleware<MyRolesMiddleware>(app.Configuration);
...

public async Task InvokeAsync(HttpContext context)
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var ci = context.User.Identity as ClaimsIdentity;
        if (ci != null)
        {
            string[] roles = new[] { "users", /*"admins"*/ };
            foreach (var role in roles) ci.AddClaim(new Claim(ClaimTypes.Role, role));
        }
    }
    await _next(context);
}

```



## Aspnetframework48 project

### local dev setup

- create a Web.local.config (git ignored) with the following content:
```xml
<appSettings>
	<add key="entraid:ClientId" value="27ab....." />
	<add key="entraid:Authority" value="https://login.microsoftonline.com/034............../v2.0" />
	<add key="entraid:RedirectUri" value="https://localhost:44391/signin-oidc" />
</appSettings>
```

### Roles

Some areas of the application, like `admin-area` and `user-area` are protected by roles using specific web.config like:
```
<configuration>
  <system.web>
    <authorization>
      <allow roles="admins"/>
      <deny users="*"/>
      <deny users="?"/>
    </authorization>
  </system.web>
</configuration>
```

The roles are added to the user claims just after the user is authenticated by Entra ID in `OpenIdConnectAuthenticationNotifications.SecurityTokenValidated` event. Then, the roles are used in the `Web.config` files of the areas to restrict access.
When an authenticated user tries to access a protected area where they don't have the required role, they are redirected to the `/NotAuthorized.aspx` page. The function `OpenIdConnectAuthenticationNotifications.RedirectToIdentityProvider` handles the redirections to the identity provider, but can be used to identify if a user is already authenticated but not allowed to enter a restriced area. The flow is:
1.	The user signs in to Entra ID, and the OWIN cookie middleware establishes an authenticated user.
2.	The user requests a protected resource. The app’s authorization check finds that they lack the required role and rejects the request. In this flow, that rejection is treated as an OpenID Connect authentication challenge.
3.	The OpenID Connect middleware is about to redirect the browser to Entra ID. Just before doing so, it calls RedirectToIdentityProvider.
4.	This specific code in the callback checks whether if this is an authentication request and whether the user is already authenticated. If both are true, it treats the challenge as an access-denied case: HandleResponse() stops the middleware’s normal redirect to Entra ID, and the explicit redirect sends the user to NotAuthorized.aspx.
5.	If the user is not authenticated, the condition is false, so the normal Entra ID sign-in flow proceeds.


## Useful Links

- https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/tree/master/1-WebApp-OIDC/1-1-MyOrg
- https://github.com/AzureAD/microsoft-identity-web/tree/master/tests/DevApps/aspnet-mvc/OwinWebApp

