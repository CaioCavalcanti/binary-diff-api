# binary-diff-api

This is a proof of concept of an RESTful API built upon ASP.NET Core

PoC proposal:

- Provide 2 http endpoints that accepts JSON base64 encoded binary data on both
  endpoints:
  - `<HOST>/v1/diff/<ID>/left`
  - `<HOST>/v1/diff/<ID>/right`
- The provided data needs to be diff-ed and the results shall be available on a third endpoint
  - `<HOST>/v1/diff/<ID>`
- The results shall provide the following info in JSON format
  - If equal return that
  - If not of equal size just return that
  - If of same size provide insight in where the diffs are, actual diffs are not needed.
    - So mainly offsets + length in the data
- Data persistency implementation is not the focus, but should implement the correct abstractions so it can be easily replaced

Assumptions were taken on how to implement this PoC, you can check it at the end under [its section at the end](#Assumptions).

# What's in it?

- API Gateway
  - ASP.NET Core 2.1
  - Ocelot
- Input Web API
  - ASP.NET Core 2.1
  - MongoDB
- Result Web API
  - ASP.NET Core 2.1
  - PostgreSQL
- Worker Console App
  - Console App on .NET Core 2.1
- RabbitMQ as Event Bus
- Docker files: `Dockerfile` and `docker-compose.yml`

# Running it

The project was built to run on Docker, so you just need to execute the command below on the repo:

```
$ docker-compose -f docker-compose.yml -f docker-compose-infrastructure.yml up --build
```

As soon as it finishes building the container, the service will be available on `http://localhost:4000/`

# Testing

1. Create a new diff

```
POST http://localhost:4000/v1/diff
```

2. Post `left` and/or `right` data using the ID you got as result from step 1

```
POST http://localhost:4000/v1/diff/<ID>
```

3. Get the results for the diff you created

```
GET http://localhost:4000/v1/diff/<ID>
```

## Unit Tests

You can execute the tests using [NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x) with the command below:

```
$ dotnet test
```

# Assumptions

- In order to use the endpoints you'll need a diff ID, as there is no way to call `/v1/diff/<ID>/left`, `/v1/diff/<ID>/right` or `/v1/diff/<ID>` if you don't actually have an ID.
- To create a new diff you'll need `POST <HOST>/v1/diff`, example:

```
# Request
POST /v1/diff HTTP/1.1
Host: <HOST>
Accept: application/json
[...]

# Response
HTTP/1.1 200 OK
Content-Type: application/json
Location: /v1/diff/c274cfba-653f-472b-b10f-b18d56a655a4
[...]

{
    "id": "26e67e07-f142-440e-b7af-dd39842c8678"
}
```

- ID type is **Guid** (i.e. `26e67e07-f142-440e-b7af-dd39842c8678`).
- You'll need to use `POST` method to provide data on `/v1/diff/<ID>/left` and `/v1/diff/<ID>/right`, example:

```
# Request
POST /v1/diff/<ID>/left HTTP/1.1
Host: <HOST>
Accept: application/json
[...]

# Response
HTTP/1.1 201 Created
Content-Type: application/json
[...]
```

- `null` and `string.Empty` are valid inputs and were taken in consideration as length 0.
- `/left` and `/right` expect the input as a base 64 encoded string sent as example below.

```
POST /v1/diff/<ID>/<left|right> HTTP/1.1
Host: <HOST>
Content-type: application/json
[...]

{
    "data": "SGVsbG93IFdvcmxkIQ=="
}
```

- Diff results are stored on a repository to reduce processing time, so we can compare data async on a different service
