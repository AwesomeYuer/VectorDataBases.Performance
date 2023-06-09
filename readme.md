﻿# Special thanks for `GFW`, `Internet` and `OpenSource`

# `Qdrant` vs `RediSearch` vs `PgVector` performance testing

- [slide/deck](Vector.DataBases.And.Performance.Awesome.Yuer.pptx)

- [manual](manual.md)

# Self Testing Results and Rank

![Self Testing Results and Rank](VectorDataBases.Performance/Assets/results.png)



# `Qdrant` non-concurrency `Vector Search`, `BenchmarkDotNet` performance testing

## Run in `docker` `qdrant` `ubuntu` VM

- `SK Http` means `Microsoft.SemanticKernel.Connectors.Memory.Qdrant.QdrantVectorDbClient`

![qdrant.50w.100w.grpc.http.local](VectorDataBases.Performance/Assets/qdrant.50w.100w.grpc.http.local.png)

![qdrant.advantage.local](VectorDataBases.Performance/Assets/qdrant.advantage.local.png)

- Reference

    https://github.com/Azure-Samples/qdrant-azure

    https://devblogs.microsoft.com/semantic-kernel/the-power-of-persistent-memory-with-semantic-kernel-and-qdrant-vector-database/

    https://devblogs.microsoft.com/semantic-kernel/qdrant/


# Docker Run `PostgreSQL` Database Server
```
docker pull ankane/pgvector

docker run --name test-pgvector -v ~/temp:/share -e POSTGRES_PASSWORD=password01! -d -p 5432:5432 ankane/pgvector

```


```sql

-- Database: pgvectors

-- DROP DATABASE IF EXISTS pgvectors;

CREATE DATABASE pgvectors
    WITH
    OWNER = sa
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;



create extension vector;


-- Table: public.embeddings

-- DROP TABLE IF EXISTS public.embeddings;
-- 11w rows
CREATE TABLE IF NOT EXISTS public.embeddings
(
    id bigserial,
    content character varying(1024) COLLATE pg_catalog."default",
    vector_id integer,
    embedding vector(1536)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.embeddings
    OWNER to sa;
-- Index: idx_ivfflat_vector_cosine_ops_on_embeddings

-- DROP INDEX IF EXISTS public.idx_ivfflat_vector_cosine_ops_on_embeddings;

CREATE INDEX IF NOT EXISTS idx_ivfflat_vector_cosine_ops_on_embeddings
    ON public.embeddings USING ivfflat
    (embedding vector_cosine_ops)
    TABLESPACE pg_default;
-- Index: idx_ivfflat_vector_ip_ops_on_embeddings

-- DROP INDEX IF EXISTS public.idx_ivfflat_vector_ip_ops_on_embeddings;

CREATE INDEX IF NOT EXISTS idx_ivfflat_vector_ip_ops_on_embeddings
    ON public.embeddings USING ivfflat
    (embedding vector_ip_ops)
    TABLESPACE pg_default;
-- Index: idx_ivfflat_vector_l2_ops_on_embeddings

-- DROP INDEX IF EXISTS public.idx_ivfflat_vector_l2_ops_on_embeddings;

CREATE INDEX IF NOT EXISTS idx_ivfflat_vector_l2_ops_on_embeddings
    ON public.embeddings USING ivfflat
    (embedding)
    TABLESPACE pg_default;


create extension pg_trgm;

SELECT strict_word_similarity('word', 'two words'), similarity('word', 'words');

-- Use EXPLAIN ANALYZE to debug performance
EXPLAIN ANALYZE SELECT * FROM embeddings ORDER BY embedding <-> '[3,1,2]' LIMIT 1;


```

# .NET 盛世环境
```sh
# https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu-2004
# Add the Microsoft package repository

wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb


# Install the SDK

sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-6.0

# Install the runtime

sudo apt-get update && \
  sudo apt-get install -y aspnetcore-runtime-6.0


sudo apt-get install -y dotnet-runtime-6.0


```




```sql
-- Table: public.wikipedia

-- DROP TABLE IF EXISTS public.wikipedia;

CREATE TABLE IF NOT EXISTS public.wikipedia
(
    id integer,
    url character varying(512) COLLATE pg_catalog."default",
    title character varying(512) COLLATE pg_catalog."default",
    text text COLLATE pg_catalog."default",
    title_vector vector(1536),
    content_vector vector(1536),
    vector_id integer
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.wikipedia
    OWNER to sa;
-- Index: idx_ivfflat_vector_cosine_ops_on_wikipedia

-- DROP INDEX IF EXISTS public.idx_ivfflat_vector_cosine_ops_on_wikipedia;

CREATE INDEX IF NOT EXISTS idx_ivfflat_vector_cosine_ops_on_wikipedia
    ON public.wikipedia USING ivfflat
    (title_vector vector_cosine_ops)
    TABLESPACE pg_default;
	
	
COPY wikipedia FROM '/MyGitHub/openai-cookbook-python/examples/data/vector_database_wikipedia_articles_embedded.csv' DELIMITER ',' CSV HEADER;
	
```

# pgvector

Open-source vector similarity search for Postgres

```sql
CREATE TABLE items (embedding vector(3));
CREATE INDEX ON items USING ivfflat (embedding vector_l2_ops);
SELECT * FROM items ORDER BY embedding <-> '[1,2,3]' LIMIT 5;
```

Supports L2 distance, inner product, and cosine distance

[![Build Status](https://github.com/pgvector/pgvector/workflows/build/badge.svg?branch=master)](https://github.com/pgvector/pgvector/actions)

## Installation

Compile and install the extension (supports Postgres 11+)

```sh
git clone --branch v0.4.1 https://github.com/pgvector/pgvector.git
cd pgvector
make
make install # may need sudo
```

Then load it in databases where you want to use it

```sql
CREATE EXTENSION vector;
```

You can also install it with [Docker](#docker), [Homebrew](#homebrew), [PGXN](#pgxn), or [conda-forge](#conda-forge)

## Getting Started

Create a vector column with 3 dimensions

```sql
CREATE TABLE items (embedding vector(3));
```

Insert values

```sql
INSERT INTO items VALUES ('[1,2,3]'), ('[4,5,6]');
```

Get the nearest neighbor by L2 distance

```sql
SELECT * FROM items ORDER BY embedding <-> '[3,1,2]' LIMIT 1;
```

Also supports inner product (`<#>`) and cosine distance (`<=>`)

Note: `<#>` returns the negative inner product since Postgres only supports `ASC` order index scans on operators

## Querying

Use a `SELECT` clause to get the distance

```sql
SELECT embedding <-> '[3,1,2]' AS distance FROM items;
```

Use a `WHERE` clause to get rows within a certain distance

```sql
SELECT * FROM items WHERE embedding <-> '[3,1,2]' < 5;
```

Note: Combine with `ORDER BY` and `LIMIT` to use an index

Get the average of vectors

```sql
SELECT AVG(embedding) FROM items;
```

## Indexing

Speed up queries with an approximate index. Add an index for each distance function you want to use.

L2 distance

```sql
CREATE INDEX ON items USING ivfflat (embedding vector_l2_ops);
```

Inner product

```sql
CREATE INDEX ON items USING ivfflat (embedding vector_ip_ops);
```

Cosine distance

```sql
CREATE INDEX ON items USING ivfflat (embedding vector_cosine_ops);
```

Indexes should be created after the table has some data for optimal clustering. Also, unlike typical indexes which only affect performance, you may see different results for queries after adding an approximate index. Vectors with up to 2,000 dimensions can be indexed.

### Index Options

Specify the number of inverted lists (100 by default)

```sql
CREATE INDEX ON items USING ivfflat (embedding vector_l2_ops) WITH (lists = 100);
```

A lower value provides better recall at the cost of speed. A good place to start is:

- `rows / 1000` for up to 1M rows
- `sqrt(rows)` for over 1M rows

### Query Options

Specify the number of probes (1 by default)

```sql
SET ivfflat.probes = 1;
```

A higher value provides better recall at the cost of speed, and it can be set to the number of lists for exact nearest neighbor search (at which point the planner won’t use the index)

Use `SET LOCAL` inside a transaction to set it for a single query

```sql
BEGIN;
SET LOCAL ivfflat.probes = 1;
SELECT ...
COMMIT;
```

### Indexing Progress

Check [indexing progress](https://www.postgresql.org/docs/current/progress-reporting.html#CREATE-INDEX-PROGRESS-REPORTING) with Postgres 12+

```sql
SELECT phase, tuples_done, tuples_total FROM pg_stat_progress_create_index;
```

The phases are:

1. `initializing`
2. `performing k-means`
3. `sorting tuples`
4. `loading tuples`

Note: `tuples_done` and `tuples_total` are only populated during the `loading tuples` phase

### Partial Indexes

Consider [partial indexes](https://www.postgresql.org/docs/current/indexes-partial.html) for queries with a `WHERE` clause

```sql
SELECT * FROM items WHERE category_id = 123 ORDER BY embedding <-> '[3,1,2]' LIMIT 5;
```

can be indexed with:

```sql
CREATE INDEX ON items USING ivfflat (embedding vector_l2_ops) WHERE (category_id = 123);
```

To index many different values of `category_id`, consider [partitioning](https://www.postgresql.org/docs/current/ddl-partitioning.html) on `category_id`.

```sql
CREATE TABLE items (embedding vector(3), category_id int) PARTITION BY LIST(category_id);
```

## Performance

To speed up queries without an index, increase `max_parallel_workers_per_gather`.

```sql
SET max_parallel_workers_per_gather = 4;
```

To speed up queries with an index, increase the number of inverted lists (at the expense of recall).

```sql
CREATE INDEX ON items USING ivfflat (embedding vector_l2_ops) WITH (lists = 1000);
```

Use `EXPLAIN ANALYZE` to debug performance.

```sql
EXPLAIN ANALYZE SELECT * FROM items ORDER BY embedding <-> '[3,1,2]' LIMIT 1;
```

## Languages

Use pgvector from any language with a Postgres client. You can even generate and store vectors in one language and query them in another.

Language | Libraries / Examples
--- | ---
C++ | [pgvector-cpp](https://github.com/pgvector/pgvector-cpp)
C# | [pgvector-dotnet](https://github.com/pgvector/pgvector-dotnet)
Elixir | [pgvector-elixir](https://github.com/pgvector/pgvector-elixir)
Go | [pgvector-go](https://github.com/pgvector/pgvector-go)
Java, Scala | [pgvector-java](https://github.com/pgvector/pgvector-java)
Julia | [pgvector-julia](https://github.com/pgvector/pgvector-julia)
Lua | [pgvector-lua](https://github.com/pgvector/pgvector-lua)
Node.js | [pgvector-node](https://github.com/pgvector/pgvector-node)
Perl | [pgvector-perl](https://github.com/pgvector/pgvector-perl)
PHP | [pgvector-php](https://github.com/pgvector/pgvector-php)
Python | [pgvector-python](https://github.com/pgvector/pgvector-python)
R | [pgvector-r](https://github.com/pgvector/pgvector-r)
Ruby | [pgvector-ruby](https://github.com/pgvector/pgvector-ruby), [Neighbor](https://github.com/ankane/neighbor)
Rust | [pgvector-rust](https://github.com/pgvector/pgvector-rust)

## Frequently Asked Questions

#### How many vectors can be stored in a single table?

A non-partitioned table has a limit of 32 TB by default in Postgres. A partitioned table can have thousands of partitions of that size.

#### Is replication supported?

Yes, pgvector uses the write-ahead log (WAL), which allows for replication and point-in-time recovery.

#### What if I want to index vectors with more than 2,000 dimensions?

Two things you can try are:

1. use dimensionality reduction
2. compile Postgres with a larger block size (`./configure --with-blocksize=32`) and edit the limit in `src/ivfflat.h`

## Reference

### Vector Type

Each vector takes `4 * dimensions + 8` bytes of storage. Each element is a single precision floating-point number (like the `real` type in Postgres), and all elements must be finite (no `NaN`, `Infinity` or `-Infinity`). Vectors can have up to 16,000 dimensions.

### Vector Operators

Operator | Description
--- | ---
\+ | element-wise addition
\- | element-wise subtraction
<-> | Euclidean distance
<#> | negative inner product
<=> | cosine distance

### Vector Functions

Function | Description
--- | ---
cosine_distance(vector, vector) → double precision | cosine distance
inner_product(vector, vector) → double precision | inner product
l2_distance(vector, vector) → double precision | Euclidean distance
vector_dims(vector) → integer | number of dimensions
vector_norm(vector) → double precision | Euclidean norm

### Aggregate Functions

Function | Description
--- | ---
avg(vector) → vector | arithmetic mean

## Additional Installation Methods

### Docker

Get the [Docker image](https://hub.docker.com/r/ankane/pgvector) with:

```sh
docker pull ankane/pgvector
```

This adds pgvector to the [Postgres image](https://hub.docker.com/_/postgres) (run it the same way).

You can also build the image manually:

```sh
git clone --branch v0.4.1 https://github.com/pgvector/pgvector.git
cd pgvector
docker build -t pgvector .
```

### Homebrew

With Homebrew Postgres, you can use:

```sh
brew install pgvector
```

### PGXN

Install from the [PostgreSQL Extension Network](https://pgxn.org/dist/vector) with:

```sh
pgxn install vector
```

### conda-forge

With Conda Postgres, install from [conda-forge](https://anaconda.org/conda-forge/pgvector) with:

```sh
conda install -c conda-forge pgvector
```

This method is [community-maintained](https://github.com/conda-forge/pgvector-feedstock) by [@mmcauliffe](https://github.com/mmcauliffe)

## Hosted Postgres

pgvector is available on [these providers](https://github.com/pgvector/pgvector/issues/54).

To request a new extension on other providers:

- Amazon RDS - follow the instructions on [this page](https://aws.amazon.com/rds/postgresql/faqs/)
- Google Cloud SQL - vote or comment on [this page](https://issuetracker.google.com/issues/265172065)
- Azure Database - vote or comment on [this page](https://feedback.azure.com/d365community/idea/7b423322-6189-ed11-a81b-000d3ae49307)
- DigitalOcean Managed Databases - vote or comment on [this page](https://ideas.digitalocean.com/app-framework-services/p/pgvector-extension-for-postgresql)
- Render - vote or comment on [this page](https://feedback.render.com/features/p/add-pgvector-extension-to-postgresql)

## Upgrading

Install the latest version and run:

```sql
ALTER EXTENSION vector UPDATE;
```

## Upgrade Notes

### 0.4.0

If upgrading with Postgres < 13, remove this line from `sql/vector--0.3.2--0.4.0.sql`:

```sql
ALTER TYPE vector SET (STORAGE = extended);
```

Then run `make install` and `ALTER EXTENSION vector UPDATE;`.

### 0.3.1

If upgrading from 0.2.7 or 0.3.0, recreate all `ivfflat` indexes after upgrading to ensure all data is indexed.

```sql
-- Postgres 12+
REINDEX INDEX CONCURRENTLY index_name;

-- Postgres < 12
CREATE INDEX CONCURRENTLY temp_name ON table USING ivfflat (column opclass);
DROP INDEX CONCURRENTLY index_name;
ALTER INDEX temp_name RENAME TO index_name;
```

## Thanks

Thanks to:

- [PASE: PostgreSQL Ultra-High-Dimensional Approximate Nearest Neighbor Search Extension](https://dl.acm.org/doi/pdf/10.1145/3318464.3386131)
- [Faiss: A Library for Efficient Similarity Search and Clustering of Dense Vectors](https://github.com/facebookresearch/faiss)
- [Using the Triangle Inequality to Accelerate k-means](https://www.aaai.org/Papers/ICML/2003/ICML03-022.pdf)
- [k-means++: The Advantage of Careful Seeding](https://theory.stanford.edu/~sergei/papers/kMeansPP-soda.pdf)
- [Concept Decompositions for Large Sparse Text Data using Clustering](https://www.cs.utexas.edu/users/inderjit/public_papers/concept_mlj.pdf)

## History

View the [changelog](https://github.com/pgvector/pgvector/blob/master/CHANGELOG.md)

## Contributing

Everyone is encouraged to help improve this project. Here are a few ways you can help:

- [Report bugs](https://github.com/pgvector/pgvector/issues)
- Fix bugs and [submit pull requests](https://github.com/pgvector/pgvector/pulls)
- Write, clarify, or fix documentation
- Suggest or add new features

To get started with development:

```sh
git clone https://github.com/pgvector/pgvector.git
cd pgvector
make
make install
```

To run all tests:

```sh
make installcheck        # regression tests
make prove_installcheck  # TAP tests
```

To run single tests:

```sh
make installcheck REGRESS=functions                    # regression test
make prove_installcheck PROVE_TESTS=test/t/001_wal.pl  # TAP test
```

To enable benchmarking:

```sh
make clean && PG_CFLAGS=-DIVFFLAT_BENCH make && make install
```

Resources for contributors

- [Extension Building Infrastructure](https://www.postgresql.org/docs/current/extend-pgxs.html)
- [Index Access Method Interface Definition](https://www.postgresql.org/docs/current/indexam.html)
- [Generic WAL Records](https://www.postgresql.org/docs/13/generic-wal.html)
