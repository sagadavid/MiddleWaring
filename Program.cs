using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.RateLimiting;
using MiddleWaring;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.Log2File();

builder.Services.AddRateLimiter(_ =>
//limit requests per window in the policy called few
_.AddFixedWindowLimiter(policyName: "limited", options =>
{
    options.PermitLimit = 3;//max request per window
    options.Window = TimeSpan.FromSeconds(10);//window duration
    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    options.QueueLimit = 2;//two req at a time
}));

builder.Services.AddRequestTimeouts(option =>
{
    option.DefaultPolicy = new RequestTimeoutPolicy { Timeout = TimeSpan.FromSeconds(5) };
    option.AddPolicy("ShortTimeoutPolicy", TimeSpan.FromSeconds(2));
    option.AddPolicy("LongTimeoutPolicy", TimeSpan.FromSeconds(10));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

//branch to ratelimit
//app.MapGet("/ratelimiting", () =>
//Results.Ok($"{DateTime.Now.Ticks.ToString()}")).RequireRateLimiting("limited");//implement the policy, refresh window, 4 req in 6sec

app.UseRequestTimeouts();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
