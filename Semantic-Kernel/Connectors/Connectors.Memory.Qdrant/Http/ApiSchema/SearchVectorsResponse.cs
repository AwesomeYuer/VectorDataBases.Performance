﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.Memory.Qdrant.Http.ApiSchema;

#pragma warning disable CA1812 // Avoid uninstantiated internal classes: Used for Json Deserialization
internal sealed class SearchVectorsResponse : QdrantResponse
{
    internal sealed class ScoredPoint
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(NumberToStringConverter))]
        public string Id { get; }

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("score")]
        public double? Score { get; }

        [JsonPropertyName("payload")]
        public Dictionary<string, object> Payload { get; set; }

        [JsonPropertyName("vector")]
        public IEnumerable<float>? Vector { get; }

        [JsonConstructor]
        public ScoredPoint(string id, double? score, Dictionary<string, object> payload, IEnumerable<float> vector)
        {
            this.Id = id;
            this.Score = score;
            this.Payload = payload;
            this.Vector = vector;
        }
    }

    [JsonPropertyName("result")]
    public IEnumerable<ScoredPoint> Results { get; set; }

    [JsonConstructor]
    public SearchVectorsResponse(IEnumerable<ScoredPoint> results)
    {
        this.Results = results;
    }

    #region private ================================================================================

    private SearchVectorsResponse()
    {
        this.Results = new List<ScoredPoint>();
    }

    #endregion
}

public class NumberToStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = string.Empty;
        var tokenType = reader.TokenType;
        if
            (
                tokenType == JsonTokenType.String
            )
        {
            result = reader.GetString();
        }
        else if
            (
                tokenType == JsonTokenType.Number
            )
        {
            reader.TryGetInt32(out var valueInt);
            result = valueInt.ToString();
        }
        else
        {
            throw new NotSupportedException($"{nameof(JsonTokenType)}.{tokenType}");
        }
        return result;
    }

    public override void Write(Utf8JsonWriter writer, string @value, JsonSerializerOptions options)
    {
        var result = string.Empty;
        if (int.TryParse(@value, out var valueInt))
        {
            result = valueInt.ToString();
        }
        writer.WriteStringValue(result);
    }
}

#pragma warning restore CA1812 // Avoid uninstantiated internal classes
