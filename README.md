# Image Repository REST API

For the Winter 2022 Shopify Developer Intern Challenge, I built a REST API with ASP.NET Core.

Follow the instructions in the How to run section below to get started with the application.

Link to the challenge: https://docs.google.com/document/d/1eg3sJTOwtyFhDopKedRD6142CFkDfWp1QvRKXNTPIOc

![alt text](https://github.com/jacobmichels/ImageRepositoryW22/blob/master/Screenshot.png "Logo Title Text 1")

## Features
- User accounts with public/private images.
- Create, read, update, and delete images.
- Bulk create and delete endpoints.
- Swagger/Open API documentation.
- JWT authentication.
- Argon2 password hashing.
- Postman tests.

## Technologies
- C#/ASP.NET Core
- Entity Framework Core
- Swagger/Open API
- SQLite

## How to run
Here is how you can get my application running on your local machine.
### Prebuilt binaries
1. [Download](https://github.com/jacobmichels/ImageRepositoryW22/releases) the latest release for your OS architecture.
2. Extract the downloaded zip file and run the ImageRepositoryW22 executable. There are no dependencies that need to be installed. You may need to set this file as executable depending on your platform. Bypass any Windows smartscreen warnings.
3. The application is now running. To view the endpoint documentation, navigate to https://localhost:5001/swagger. (swagger may not display properly in Firefox, try Chrome)

## Tests

The tests are in the form of a Postman collection and can be downloaded on the [releases page](https://github.com/jacobmichels/ImageRepositoryW22/releases).

### Running the tests
0. Install Postman and download the latest test collection on the [releases page](https://github.com/jacobmichels/ImageRepositoryW22/releases). Download the images from the Images folder in the repository root, and place them in your Postman working directory. You can find your Postman working directory in the settings menu. (On Windows, my working directory is C:\Users\jacob\Postman\files)
1. Start Postman and import the test collection with the import button.
2. Ensure the Image Repository application is running.
3. Examine the tests to understand what they are testing, then when ready, click either of the three flows then click the run button to run all the tests within that flow. The requests under the "Requests" folder are not meant to be run in the manner, rather they are sample requests for each endpoint. The tests clean up after themselves, so feel free to run them more than once. If you find you've messed up the state of the database, you can get back to a clean slate by closing the application, and deleting the app.db file, and starting the app again.

## Swagger

To view the API endpoint documentation, with the app running, navigate to https://localhost:5001/swagger.

Here you will find each endpoint along with it's http method and brief description of it's function. You can click any endpoint to reveal more information about it, such as it's input and what it returns.

You can also use swagger to send http requests to the API, although it is a little clunkier than using a tool like Postman. You can call any endpoint by clicking on it, then clicking the "try it out" button, filling in any parameters or request body, then clicking execute. If an endpoint requires authorization, you can input a JWT to be passed with each request by clicking the "Authorize" button at the top right of the page, pasting your JWT, and clicking Authorize.
