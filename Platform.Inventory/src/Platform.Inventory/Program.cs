
using Platform.Code.MassTransit;
using Platform.Code.MongoDB;
using Platform.Inventory.Entities;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Register MongoDB and repositories
builder.Services.AddMongo().AddMongoRepository<InventeryItem>("inventoryItems")
.AddMongoRepository<CaterlogItem>("caterlogItems")
.AddMassTransitWithRabbitMq();

NewMethod(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void NewMethod(WebApplicationBuilder builder)
{
    Random jilter = new Random();
    // HTTP Client configuration
    var baseAddress = builder.Configuration.GetSection("CaterlogClient:BaseAddress").Value;
    if (string.IsNullOrEmpty(baseAddress))
    {
        throw new ArgumentNullException(nameof(baseAddress), "The BaseAddress cannot be null or empty.");
    }
    builder.Services.AddHttpClient<CaterlogClient>(client =>
    {
        client.BaseAddress = new Uri(baseAddress);
    })
    .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
        5,
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
        + TimeSpan.FromMilliseconds(jilter.Next(0, 1000)),
        onRetry: (outcome, timespan, retryAttempt) =>
        {
            Console.WriteLine($"Delaying for {timespan.TotalSeconds} seconds, then making retry #{retryAttempt}");
        }
    ))
    .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
        3,
        TimeSpan.FromSeconds(15),
        onBreak: (outcome, timespan) =>
        {
            Console.WriteLine($"Open  Circite for {timespan.TotalSeconds} seconds...");
        },
        onReset: () =>
        {
            Console.WriteLine($"Closing the Circite.... ");
        }
    ))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
}