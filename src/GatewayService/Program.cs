var builder = WebApplication.CreateBuilder(args);

//add this
builder.Services.AddReverseProxy()
.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();


app.MapReverseProxy();

app.Run();
