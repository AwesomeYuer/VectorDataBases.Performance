﻿namespace VectorDataBases.Performance;

public static class GlobalManager
{
    // https://www.npgsql.org/doc/connection-string-parameters.html#pooling
    // https://stackoverflow.com/questions/44272459/postgres-npgsql-connection-pooling
    public static string PostgreSQLConnectionString =
        "Host=localhost;Database=pgvectors;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=30;User Id=sa;Password=2022db@Qwer"
        //"Host=localhost;Database=pgvectors;User Id=sa;Password=2022db@Qwer"
        ;
    public static string SelfHostRedisConnectionString = "localhost, password=2022db@Qwer";
    public static string AzureRedisConnectionString = "awesomeyuer.eastus.redisenterprise.cache.azure.net:10000, password=zzzz6Ccc4qL8kq7Wsp7X2aNObRiXN28MnDxaKm0c=";

    public static string SelfHostQdrantGrpcConnectionString = "http://localhost:6334";

    public static string SelfHostQdrantHttpConnectionString = "http://localhost:6333";

    public static string SelfHostMilvusGrpcConnectionString = "kc-misc-001-vm.koreacentral.cloudapp.azure.com";


    public static string SelfHostChromaHttpConnectionString = "http://kc-misc-001-vm.koreacentral.cloudapp.azure.com:8000";
}
