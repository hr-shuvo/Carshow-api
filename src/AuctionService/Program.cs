using AuctionService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
    opt.UseNpgsql(connectionString);
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);
        o.UsePostgres();
        o.UseBusOutbox();
    });
    
    x.UsingRabbitMq((ctx, cfg) =>
    {
        // cfg.Host("rabbitmq");
        cfg.ConfigureEndpoints(ctx);
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.MapControllers();

try
{
    DbInitializer.InitDb(app);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

app.Run();
