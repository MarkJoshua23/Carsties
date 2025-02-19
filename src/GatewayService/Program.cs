using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

//add this
builder.Services.AddReverseProxy()
.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


//authentcation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    //put the authority ro the identity server we made
    //ASP.NET sends the token to IdentityServer to verify if it’s valid
    options.Authority = builder.Configuration["IdentityServiceUrl"];
    //to allow http
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters.ValidateAudience = false;

    // Maps the 'username' claim to User.Identity.Name
    options.TokenValidationParameters.NameClaimType = "username";
});


var app = builder.Build();

app.MapReverseProxy();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
