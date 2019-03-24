# binary-diff-api

This is a proof of concept of a scalable RESTful API built upon ASP.NET Core

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

- API Gateway (`http://localhost:4000/`)
  - Serves as gateway to centralize the different endpoints and APIs on `/v1/diff`
  - Built upon ASP.NET Core 2.1 and Ocelot
- Input Web API (`http://localhost:5000/`)
  - Adds new diffs and inputs for both positions and notifies
  - Built upon ASP.NET Core 2.1 and MongoDB
- Result Web API (`http://localhost:6000/`)
  - Receives new diff results from queue in background and provide them on an API endpoint
  - Built upon ASP.NET Core 2.1 and PostgreSQL
- Worker Console App
  - Process new inputs and publishing the results on queue
  - Built upon .NET Core 2.1
- RabbitMQ (`http://localhost:15672/`)
  - Serves as event bus
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
----------------------------------------
HTTP/1.1 201 Created
{
    "id": "a10944b1-2b90-4645-963d-c36a5e8f7569"
}
```

2. Post `left` and/or `right` data using the ID you got as result from step 1

```
POST http://localhost:4000/v1/diff/<ID>/<left|right>
{
    "data": "SGVsbG93IFdvcmxkIQ=="
}
----------------------------------------
HTTP/1.1 201 Created
{
    "id": "<INPUT_ID>",
    "position": "Left|Right",
    "diffId": "<ID>",
    "timestamp": "<INPUT_CREATION_DATE>"
}
```

3. Get the results for the diff you created

```
GET http://localhost:4000/v1/diff/<ID>
----------------------------------------
HTTP/1.1 200 OK
{
    "id": "<RESULT_ID>",
    "result": "Different|Equal|LeftIsLarger|RightIsLarger",
    "timestamp": "<RESULT_CREATION_DATE>",
    "differences": [
        {
            "offset": 0,
            "length": 6
        }
    ]
}
```

## Unit and Integration Tests

You can execute the unit tests using [NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x) with the command below:

```
$ dotnet test
```

## Integration Tests

Before running the integration tests, you'll need to start the service containers on Docker as shown on the section [Running It](#Running-It):

After getting all up and running you can just run the tests on project `BinaryDiff.IntegrationTests`:

```
$ Tests\BinaryDiff.IntegrationTests> dotnet test
```

# Assumptions

- In order to use the endpoints you'll need a diff ID, as there is no way to call `/v1/diff/<ID>/left`, `/v1/diff/<ID>/right` or `/v1/diff/<ID>` if you don't actually have an ID.
- To create a new diff you'll need `POST <HOST>/v1/diff`, example:

```
# Request
POST /v1/diff HTTP/1.1
Host: <HOST>
Accept: application/json
----------------------------------------
HTTP/1.1 200 OK
Content-Type: application/json
Location: c274cfba-653f-472b-b10f-b18d56a655a4
{
    "id": "26e67e07-f142-440e-b7af-dd39842c8678"
}
```

- ID type is **Guid** (i.e. `26e67e07-f142-440e-b7af-dd39842c8678`)
- You'll need to use `POST` method to provide data on `/v1/diff/<ID>/left` and `/v1/diff/<ID>/right`, example:

```
POST /v1/diff/fb386f98-d475-49d5-b027-658d7efe8ec1/left HTTP/1.1
Host: <HOST>
Accept: application/json
{
    "data": "SGVsbG93IFdvcmxkIQ=="
}
----------------------------------------
HTTP/1.1 201 Created
Content-Type: application/json
Location: 5c96f39cbb9fda000111f7f5
{
    "id": "5c96f39cbb9fda000111f7f5",
    "position": "Left",
    "diffId": "fb386f98-d475-49d5-b027-658d7efe8ec1",
    "timestamp": "2019-03-24T03:03:56.5541059Z"
}
```

- `null` and `string.Empty` are valid inputs and were taken in consideration as length 0
- `/left` and `/right` expect the input as a base 64 encoded string sent as example below.

```
POST /v1/diff/<ID>/<left|right> HTTP/1.1
Host: <HOST>
Content-type: application/json
{
    "data": "SGVsbG93IFdvcmxkIQ=="
}
```

- Diff results are stored on a repository to reduce processing time, so we can compare data async on a different service, called `Worker`
- When a new input is registered on `Input Web API` (either `/left` or `/right`), a new input event is published in RabbitMQ
- `Worker` is a simples console app that subscribes to new input events on RabbitMQ and process them as
- After processing the results, `Worker` publishes a new diff result to RabbitMQ
- `Result Web API` subscribes to new diff results events, registering them on the repository as they arrive

# Improvements opportunities

- Use Kubernetes to orchestrate and scale containers
- Use redis to cache results in memory
- Add endpoints to get details for input and diff result by id
- Implement HATEOAS
- Implement load/stress tests
