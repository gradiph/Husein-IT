using Microsoft.AspNetCore.Http;

namespace CommonHttp
{
    public class ApiResponseBuilder
    {
        private HttpRequest _httpRequest;
        private int _statusCode;
        private string _message;
        private object _response;

        public ApiResponseBuilder(HttpRequest httpRequest, int statusCode, object response, string message = "")
        {
            _httpRequest = httpRequest;
            _statusCode = statusCode; 
            _message = message;
            _response = response;
        }
        
        public IResult Build()
        {
            if (_statusCode == 200)
            {
                return Results.Ok(_response);
            }
            else if (_statusCode == 201)
            {
                return Results.Created(_message, _response);
            }
            else if (_statusCode == 204)
            {
                return Results.NoContent();
            }
            else if (_statusCode == 422)
            {
                return Results.UnprocessableEntity(_message);
            }
            else
            {
                return Results.Problem(_message, _httpRequest.Path, _statusCode, "Error Other");
            }
        }
    }
}