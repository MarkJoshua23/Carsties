using System;
using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;
//iprofileservice to add details in jwt
//click lightbulb => implement interface
public class CustomProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    //we need usermanager to access info about the user and use that to put into the jwt
    public CustomProfileService(UserManager<ApplicationUser> _userManager)
    {
        this._userManager = _userManager;
    }

    //GetProfileDataAsync will add additional information to jwt
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        //get user info by id in context.subject
        var user = await _userManager.GetUserAsync(context.Subject);
        //store the fullname claim that is previously stored when the account is made
        var existingClaims = await _userManager.GetClaimsAsync(user);

        var claims = new List<Claim>{
            //additional claim to be sent
            new Claim("username",user.UserName)
        };

        //add UserName to the jwt claims
        context.IssuedClaims.AddRange(claims);
        //add the existing claims that is recorded in identity server(Full Name)
        context.IssuedClaims.Add(existingClaims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name));
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        return Task.CompletedTask;
    }
}
