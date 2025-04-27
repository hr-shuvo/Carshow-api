using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient<AuctionServiceHttpClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

// app.UseHttpsRedirection();

app.MapControllers();

try
{
    await DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
}


app.Run();

