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

Assumptions were taken on how to implement this PoC, you can check it at the end under [its section at the end](#Assumptions).

# What's in it?

- ASP.NET Core 2.1

```
TODO: give more details on what's in it and why
```

# Running it

It was built to run on Docker (~~because runs on my machine is not an excuse~~), so you just need to run on the repo:

```
$ docker-compose up
```

As soon as it finishes building the containers the service will be available on `<HOST>`, but you can configure a different port on `Dockerfile` if needed.

The API documentation is available on Swagger.

# Testing

```
$ dotnet test
```

```
TODO: give more detail on how to execute unit, integration tests or/and both
```

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

- The difference result is based on **bas64 encoded** binary data provided.
- If not timestamp provided on query params when getting diff results, it will compare the latest data on both sides.