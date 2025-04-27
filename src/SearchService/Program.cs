using Polly;
using Polly.Extensions.Http;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient<AuctionServiceHttpClient>()
    .AddPolicyHandler(GetRetryPolicy());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

// app.UseHttpsRedirection();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() =>
{
    _ = Task.Run(async () =>
    {
        try
        {
            await DbInitializer.InitDb(app);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
        }
    });

});


app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    => HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));

