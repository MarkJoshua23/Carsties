using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Register
{
    //this will allow anonymous to access the register page 
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        public void OnGet()
        {
        }
    }
}
