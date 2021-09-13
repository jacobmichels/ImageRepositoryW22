# Image Repository REST API

For the Winter 2022 Shopify Developer Intern Challenge, I built a REST API with ASP.NET Core.

Link to challenge: https://docs.google.com/document/d/1eg3sJTOwtyFhDopKedRD6142CFkDfWp1QvRKXNTPIOc

## Features
- User accounts with public/private images.
- Create, read, update, and delete images.
- Bulk create and delete endpoints.
- Search for text in uploaded images with OCR.
- Swagger API documentation.
- JWT authentication.
- Argon2 password hashing.

## How to run
There are a few different ways you can get my application running.
### Prebuilt binaries
1. [Download](https://github.com/jacobmichels/ImageRepositoryW22/releases) the latest release for your computer's architecture.
2. Extract the file and run ImageRepositoryW22.exe. There are no dependencies that need to be installed.

### Docker
TODO

### Build from source
TODO

## Tests

The tests are in the form of a Postman collection and can be downloaded on the [releases page](https://github.com/jacobmichels/ImageRepositoryW22/releases).

### Running the tests
0. Install Postman and download the latest collection on the [releases page](https://github.com/jacobmichels/ImageRepositoryW22/releases).
1. Import the  collection into Postman. Open the collection and inspect the requests and tests for each request.
2. Start the ImageRepository server by running ImageRepositoryW22.exe downloaded earlier. Check the logs in the terminal and ensure it is running at https://localhost:5001.
