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

- API based on ASP.NET Core 2.1
- Unit and integration tests on XUnit
- Docker files: `Dockerfile` and `docker-compose.yml`

# Running it

The project was built to run on Docker (~~because runs on my machine is not an excuse~~), so you just need to execute the command below on the repo:

```
$ docker-compose up
```

As soon as it finishes building the container, the service will be available on `http://localhost:8000/v1/<RESOURCE_PATH>` (you can configure a different port on `Dockerfile` if needed).

If Docker is not an option in your case, you can also run it using [NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x) with the following commands:

```
$ dotnet restore
$ dotnet run --project BinaryDiff.API\BinaryDiff.API.csproj
```

Swagger is configured and available on root `http://localhost:8000/`.

# Testing

`Dockerfile` is configured to execute tests when building the image, you can check the results on the logs.

You can also run the tests using [NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x) with the command below:

```
$ dotnet test
```

If you are interested on code coverage you can use params `/p:CollectCoverage=true /p:Exclude="[xunit*]*"` (targeting [Coverlet](https://github.com/tonerdo/coverlet))

```
$ dotnet test /p:CollectCoverage=true /p:Exclude="[xunit*]*"
```

`/p:Exclude="[xunit*]*"` is needed to avoid an known issue in Coverlet ([Latest NuGet package no longer works #359](https://github.com/tonerdo/coverlet/issues/359))

## Stressing out

```
TODO: if spare time... build a client app to stress out the API
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
- You'll need to use `POST` HTTP method to provide data on `/v1/diff/<ID>/left` and `/v1/diff/<ID>/right`, example:

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
- `/left` and `/right` expect the input as a base 64 encoded string.
- Differences field is optional and will be returned only if diff result is `Different`. Examples:

```
{
    "result": "Equal" // or "LeftIsLarger", or "RightIsLarger"
}

// OR

{
    "result": "Different",
    "differences": {
        "0": 3,
        "151": 2,
        "380": 4,
        "463": 5
    }
}
```

- If not timestamp provided on query params when getting diff results, it will compare the latest data on both sides.
- Timestamps always in UTC
