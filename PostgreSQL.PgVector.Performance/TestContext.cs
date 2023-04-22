﻿using BenchmarkDotNet.Attributes;
using Npgsql;
using Pgvector;
using Pgvector.Npgsql;
using System.Data;
using System.Data.Common;

namespace PostgreSQL.PgVector.Performance;

public class TestContext
{
    [Benchmark()]
    public async Task ProcessAsync()
    {
        //Thread.Sleep(200);
        //await Task.Delay(200);
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(GlobalManager.ConnectionString);
        dataSourceBuilder.UseVector();

        using var npgsqlDataSource = dataSourceBuilder.Build();
        using var connection = await npgsqlDataSource.OpenConnectionAsync();
        //Console.WriteLine("Opened");

        var floats = new float[1536]
                                .Select
                                    (
                                        (x) =>
                                        {
                                            return
                                                (float) new Random().NextDouble();
                                        }
                                    )
                                .ToArray();

        var pgVector = new Vector(floats);
        var limit = 5;
        var sql = @$"
WITH
T
as
(
    SELECT
        *
        , embedding <-> $1::vector  as ""EuclideanL2Distance""
        , embedding <=> $1::vector  as ""CosineDistance""
    FROM
        embeddings AS a
)
SELECT
    *
    , (1 - a.""CosineDistance"")    as ""CosineSimilarity""
FROM
    T a
order by
    --""EuclideanL2Distance""
    ""CosineDistance"" 
                --DESC
LIMIT $2;
";
        using var npgsqlCommand = new NpgsqlCommand();
        npgsqlCommand.Connection = connection;
        npgsqlCommand.CommandText = sql;
        npgsqlCommand.Parameters.AddWithValue(pgVector);
        npgsqlCommand.Parameters.AddWithValue(limit);

        using
            (
                DbDataReader dataReader =
                                await npgsqlCommand.ExecuteReaderAsync()
            )
        {
            var j = 0;
            while (await dataReader.ReadAsync())
            {
                IDataRecord dataRecord = dataReader;
                var fieldsCount = dataRecord.FieldCount;
                for (var i = 0; i < fieldsCount; i++)
                {
                    _ = dataRecord[dataReader.GetName(i)];
                }
                j++;
            }
            //Console.WriteLine(j);
        }
    }

    //[Benchmark]
    //public async Task ProcessAsync2()
    //{

    //    //await using
    //    //    (
    //    //        DbDataReader dataReader =
    //    //                        await _npgsqlCommand.ExecuteReaderAsync()
    //    //    )
    //    //{
    //    //    while (await dataReader.ReadAsync())
    //    //    {
    //    //        IDataRecord dataRecord = dataReader;
    //    //        var fieldsCount = dataRecord.FieldCount;
    //    //        for (var i = 0; i < fieldsCount; i++)
    //    //        {
    //    //            _ = dataRecord[dataReader.GetName(i)];
    //    //        }
    //    //    }
    //    //}
    //}
}
