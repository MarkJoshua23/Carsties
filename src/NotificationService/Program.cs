using MassTransit;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(x =>
{

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("nt", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        //define the host
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            //define the username, if no env var then default to "guest"
            host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });
        cfg.ConfigureEndpoints(context);
    });
});
var app = builder.Build();

app.Run();
