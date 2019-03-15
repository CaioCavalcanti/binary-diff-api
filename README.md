# binary-diff-api

This is a proof of concept of an RESTful API built upon ASP.NET Core

PoC proposal:
- Provide 2 http endpoints that accepts JSON base64 encoded binary data on both
endpoints:
  - `<HOST>/v1/diff/<ID>/left`
  - `<HOST>/v1/diff/<ID>/right`
- The provided data needs to be diff-ed and the results shall be available on a third endpoint
  - `<HOST>/v1/diff<ID>`
- The results shall provide the following info in JSON format
  - If equal return that
  - If not of equal size just return that
  - If of same size provide insight in where the diffs are, actual diffs are not needed.
    - So mainly offsets + length in the data

# What's in it?
- ASP.NET Core 2.1

```TODO: give more details on what's in it and why```

# Running it
It was built to run on Docker (~~because runs on my machine is not an excuse~~), so you just need to run on the repo:

```$ docker-compose up```

As soon as it finishes building the containers the service will be available on `<HOST>`, but you can configure a different port on `Dockerfile` if needed.

The API documentation is available on Swagger.

# Testing
```$ dotnet test```

```TODO: give more detail on how to execute unit, integration tests or/and both```

## Stressing out
```TODO: if spare time... build a client app to stress out the API```