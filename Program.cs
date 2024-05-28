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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//3 middleware components implements own logic and passes next, statements run according to scopes inner order, nemlig: after 2 line 1, line 2, after 1, line 1, line 2
app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();

    logger.LogInformation($"request host : {context.Request.Host}");
    logger.LogInformation("Before Next 0");

    await next(context);

    logger.LogInformation("After Next 0");
    logger.LogInformation($"response status kode: {context.Response.StatusCode}");
});

app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"ClientName HttpHeader in Middleware 1: {context.Request.Headers["ClientName"]}");
    logger.LogInformation($"Add a ClientName HttpHeader in Middleware 1");
    context.Request.Headers.TryAdd("ClientName", "Machos");
    logger.LogInformation("Before Next 1");
    await next(context);
    logger.LogInformation("After Next 1");
    logger.LogInformation($"Response StatusCode in Middleware 1: {context.Response.StatusCode}");
});

app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"ClientName HttpHeader in Middleware 2: {context.Request.Headers["ClientName"]}");
    logger.LogInformation("Before Next 2");
    context.Response.StatusCode = StatusCodes.Status202Accepted;
    await next(context);
    logger.LogInformation("After 2");
    logger.LogInformation($"Response StatusCode in Middleware 2: {context.Response.StatusCode}");
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
