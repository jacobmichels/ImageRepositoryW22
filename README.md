# Image Repository REST API

For the Winter 2022 Shopify Developer Intern Challenge, I built a REST API with ASP.NET Core.

Link to the challenge: https://docs.google.com/document/d/1eg3sJTOwtyFhDopKedRD6142CFkDfWp1QvRKXNTPIOc

## Features
- User accounts with public/private images.
- Create, read, update, and delete images.
- Bulk create and delete endpoints.
- Swagger API documentation.
- JWT authentication.
- Argon2 password hashing.
- Postman tests.

## How to run
There are a few different ways you can get my application running.
### Prebuilt binaries
1. [Download](https://github.com/jacobmichels/ImageRepositoryW22/releases) the latest release for your OS architecture.
2. Extract the file and run the ImageRepositoryW22 executable. There are no dependencies that need to be installed. You may need to set this file as executable depending on your platform. Bypass any Windows smartscreen warnings.
3. The application is now running. To view the endpoint documentation, navigate to https://localhost:5001/swagger. (swagger may not display properly in Firefox, try Chrome)

### Docker
TODO

### Build from source
TODO

## Tests

The tests are in the form of a Postman collection and can be downloaded on the [releases page](https://github.com/jacobmichels/ImageRepositoryW22/releases).

### Running the tests
0. Install Postman and download the latest test collection on the [releases page](https://github.com/jacobmichels/ImageRepositoryW22/releases). Download the images from the Images folder in the repository root, and place them in your Postman working directory. You can find your Postman working directory in the settings menu. (On Windows, my working directory is C:\Users\jacob\Postman\files)
1. Start Postman and import the test collection with the import button.
2. Ensure the Image Repository application is running.
3. Examine the tests to understand what they are testing, then when ready, click either of the three flows then click the run button to run all the tests within that flow. The requests under the "Requests" folder are not meant to be run in the manner, rather they are sample requests for each endpoint. The tests clean up after themselves, so feel free to run them more than once. If you find you've messed up the state of the database, you can get back to a clean slate by closing the application, and deleting the app.db file, and starting the app again.
