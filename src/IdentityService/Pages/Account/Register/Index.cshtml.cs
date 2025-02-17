using System.Security.Claims;
using IdentityModel;
using IdentityService.Models;
using IdentityService.Pages.Account.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Register
{
    //this will allow anonymous to access the register page 
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        //inject usermanager
        public Index(UserManager<ApplicationUser> _usermanager)
        {
            this._usermanager = _usermanager;
        }

        //the receiver of form values when submitted using POST
        //just like useState
        //this only accepts the inputs so the returnURL is still null
        [BindProperty]
        public RegisterViewModel Input { get; set; }

        [BindProperty]
        public bool RegisterSuccess { get; set; }

        //GET
        //Default when url is entered
        //no Task because its not async
        public IActionResult OnGet(string returnUrl)
        {
            //this will set the missing null
            Input = new RegisterViewModel
            {
                ReturnUrl = returnUrl,
            };

            //return the page to the user
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            //read the button value
            //if they click cancel instead of register
            if (Input.Button != "register") return Redirect("~/");
            //check if the values passed validation like required stringlength, regex, etc
            if (ModelState.IsValid)
            {
                //applicationuser is from identity it needs uname,email,confirmed
                var user = new ApplicationUser
                {
                    UserName = Input.Username,
                    Email = Input.Email,
                    EmailConfirmed = true
                };
                //make an account based on the given credentials
                var result = await _usermanager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    //add claim to the user(user info)
                    await _usermanager.AddClaimsAsync(user, new Claim[]{
                        new Claim(JwtClaimTypes.Name , Input.FullName)
                    });

                    RegisterSuccess = true;
                }
            }
            return Page();
        }
    }
}
