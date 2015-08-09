# Web Service Mock

### What is it?
A self hosted HTTP server in a .NET C# console application.  
It listens to requests on your local machine, and returns pre-defined responses over HTTP.  
You can set this up in such as way that it mocks out third party services (i.e. your code won't know the difference).  

### When do I use this?
You're writing a service that integrates with another service that isn't built yet.  
One option is to stub it out in your code, or you can enter in the expected responses as rules in this application, and have it running just like the real environment.  
It's also useful for running your service in sandbox mode, you don't necessarily want to call third party services while running a sandbox request, so this can take over.  
Also while in development, the service you're integrating with might not have a sandbox, or might be expensive to run requests through, you can use this instead.  

### Remarks
The ready-to-run program is in the deliverables folder, and is named WebServiceMock.exe.  
There are two configurable settings found in the file WebServiceMock.exe.config in the same folder as the exe.  
"Environment:BaseUrl" is the url the server will listen on, the default value is http://localhost:8080.  
"Defaults:StatusCode" is the status code the server will respond with when a path from ~/api/mock/* is requested, the default value is 404.  
If you want all requests to be valid, you might change this value to 200.  

All custom rules are saved as JSON in a file called rules.txt in the same folder as the console application.  
It starts off with some example rules so you can see how it works.  
Once you run the exe, you can browse to http://localhost:8080/api/mock/users, and see an example JSON response you might use with your service.  

### Examples
Since this is meant to run on a local box, I don't have it running online anywhere.  
But please check out the screens folder for some screenshots I took.  

### I still don't get it!
Lets says you're integrating with an api, this api can create and get users.  
The host is https://api.com.  

The relative url to create a user is /users (via a POST).  
The relative url to get a user is /users/{id} (via a GET).  
The relative url to get all users is /users (via a GET).  

So we create three rules in our mock application, one for each of the above.  
For the POST to /users, we return a statuscode of 204.  
For the GET to /users/1 we return whatever object (JSON, or XML) the service would normally return.  
For the GET to /users we return an array of the response of a single user (i.e. many users).  

In your service, you replace https://api.com with http://localhost:8080, and make all the api calls.  
You have now mocked out the third party service, and your request - response time is now super fast too.  

This software is released under the [MIT License](http://opensource.org/licenses/MIT).