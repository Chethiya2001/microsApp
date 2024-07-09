
using MassTransit;
using Platform.Caterlog.Service.Entity;
using Platform.Code.MassTransit;
using Platform.Code.MongoDB;
using Platform.Code.Settings;



var builder = WebApplication.CreateBuilder(args);

// Load service settings from configuration
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSetting)).Get<ServiceSetting>();

builder.Services.Configure<ServiceSetting>(builder.Configuration.GetSection(nameof(ServiceSetting)));

// Register MongoDB and repositories
builder.Services.AddMongo().AddMongoRepository<Item>("items")
.AddMassTransitWithRabbitMq();


builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



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
