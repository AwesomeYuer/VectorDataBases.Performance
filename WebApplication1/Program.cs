using PostgreSQL.PgVector.Performance;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app
    .MapGet
        (
              "/pgsql"
            , async () =>
            {
                await new TestContext().PostgreSQLProcessAsync();
                return "ok";
            }
        )
    .WithName("pgsql");
app
    .MapGet
        (
              "/wikipedia"
            , async () =>
            {
                await new TestContext().WikipediaPostgreSQLProcessAsync();
                return "ok";
            }
        )
    .WithName("wikipedia");

app.Run();

