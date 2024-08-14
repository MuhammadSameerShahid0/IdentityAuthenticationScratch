namespace IdentityAuthWithScratch
{
    // Define a class for custom authentication middleware
    public class CustomAuthenticationMiddleware
    {
        // Field to store the next middleware in the pipeline
        private readonly RequestDelegate _next;

        // Constructor to initialize the middleware with the next RequestDelegate
        public CustomAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next; // Assign the next middleware to the private field
        }

        // Method that gets called for each request to handle authentication
        public async Task InvokeAsync(HttpContext context)
        {
            // Custom authorization logic here
            bool isAuthorized = CheckAuthorization(context); // Call the method to check authorization

            if (!isAuthorized) // If the user is not authorized
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized; // Set the response status code to 401
                context.Response.ContentType = "application/json"; // Set the response content type to JSON

                // Create a custom response object
                var customResponse = new
                {
                    status = 401, // Status code
                    message = "Unauthorized. Please Provide Valid Credentials" // Custom message
                };

                // Serialize the custom response object to JSON and write it to the response body
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(customResponse));
                return; // Short-circuit the pipeline, preventing further middleware execution
            }

            // If the user is authorized, pass the request to the next middleware in the pipeline
            await _next(context);
        }

        // Private method to check authorization
        private bool CheckAuthorization(HttpContext context)
        {
            // Implement your authorization logic here
            // For example, check for a specific header or token

            // Simulate unauthorized for this example
            return false; // Always return false to simulate an unauthorized request
        }
    }
}