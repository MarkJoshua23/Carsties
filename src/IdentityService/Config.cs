using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            //Used for Authentication
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {

            new ApiScope("auctionApp", "Auction app full access"),
        };

    public static IEnumerable<Client> Clients(IConfiguration config) =>
        new Client[]
        {
            new Client{
                ClientId = "postman",
                ClientName = "Postman",
                //allow user to request id and profile
                AllowedScopes =  { "openid", "profile", "auctionApp" },
                RedirectUris = {"https://www.getpostman.com/oauth2/callback"},
                ClientSecrets = new [] {new Secret("NotSecret".Sha256())},
                //password grant allows auth to service by password which is dangerous
                AllowedGrantTypes = {GrantType.ResourceOwnerPassword}
            },
            new Client{
                ClientId= "nextApp",
                ClientName= "nextAppp",
                ClientSecrets={new Secret("secret".Sha256())},
                //requires authorization code
                AllowedGrantTypes= GrantTypes.CodeAndClientCredentials,
                //additional security
                //for mobile app make it true
                RequirePkce=false,
                //this is auth js convention
                RedirectUris={config["ClientApp"]+"/api/auth/callback/id-server"},
                //for refresh token
                AllowOfflineAccess=true,
                AllowedScopes =  { "openid", "profile", "auctionApp" },
                //the default is 1 hour
                //make it one month
                AccessTokenLifetime = 3600*24*30,
                AlwaysIncludeUserClaimsInIdToken=true
            }
        };
}
