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

//multiple in one 
//subrequest
app.Map("/lottery", app =>
{
    var random = new Random();
    var luckyNumber = random.Next(1, 6);

    //first condition
    app.UseWhen(context => context.Request.QueryString.Value == $"?{luckyNumber.ToString()}", app =>
    {
        //done, break the branch, rejoin to main pipeline
        app.Run(async context =>
        {
            //match
            await context.Response.WriteAsync($"You win! You got the lucky number {luckyNumber}!");
        });
    });

    //not matched, second condition has 2 subs
    app.UseWhen(context => string.IsNullOrWhiteSpace(context.Request.QueryString.Value), app =>
    {
        //sub 1
        app.Use(async (context, next) =>
        {
            // enable new entry/"number" 
            var number = context.Request.Query["number"];
            //if user didnt provide an input
            if (string.IsNullOrEmpty(number))
            {
                number = random.Next(1, 6).ToString();
                context.Request.Headers.TryAdd("number", number);
            }
            await next(context);
        });

        //sub 2, takes number from sub 1, rejoin pipeline when done
        app.MapWhen(context => context.Request.Headers["number"] == luckyNumber.ToString(), app =>
        {
            //terminate, done
            app.Run(async context =>
            {
                await context.Response.WriteAsync($"You win! You got the lucky number {luckyNumber}!");
            });
        });
    });
    //default middleware
    app.Run(async context =>
    {
        var number = "";
        if (context.Request.QueryString.HasValue)
        {
            number = context.Request.QueryString.Value?.Replace("?", "");
        }
        else
        {
            number = context.Request.Headers["number"];
        }
        await context.Response.WriteAsync($"Your number is {number}. Try again!");
    });
});

app.Run(async context =>
{
    await context.Response.WriteAsync($"Use the /lottery URL to play. You can choose your number with the format /lottery?1.");
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
