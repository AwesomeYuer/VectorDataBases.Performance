using VectorDataBases.Performance;

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
              "/pgsql-ivfflat-cosine-11w"
            , async () =>
            {
                await new TestContext().PgVector_IvfflatVectorCosine_index_Cosine_11w_ProcessAsync();
                return "pgsql-ivfflat-cosine-11w";
            }
        );
    
app
    .MapGet
        (
              "/pgsql-ivfflat-cosine-25k"
            , async () =>
            {
                await new TestContext().PgVector_IvfflatVectorCosine_index_Cosine_25k_ProcessAsync();
                return "pgsql-ivfflat-cosine-25k";
            }
        );

app
    .MapGet
        (
              "/redisearch-selfhost-flat-cosine-25k"
            , async () =>
            {
                await new TestContext().RediSearch_FLAT_index_Cosine_25k_ProcessAsync();
                return "redisearch-selfhost-flat-cosine-25k";
            }
        );
app
    .MapGet
        (
              "/redisearch-azure-flat-cosine-25k"
            , async () =>
            {
                await new TestContext().AzureRediSearch_25k_ProcessAsync();
                return "redisearch-azure-flat-cosine-25k";
            }
        );

app
    .MapGet
        (
              "/redisearch-selfhost-hnsw-cosine-225k"
            , async () =>
            {
                await new TestContext().RediSearch_HNSW_index_Cosine_225k_ProcessAsync();
                return "redisearch-selfhost-hnsw-cosine-225k";
            }
        );

app.Run();
