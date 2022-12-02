using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DocumentsWebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        // NOT CURRENTLY IN USE.
        //public async Task OnGet(string redirectUri)
        //{
        //    await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new
        //    AuthenticationProperties { RedirectUri = redirectUri });
        //}
    }
}
