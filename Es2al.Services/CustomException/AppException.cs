namespace Es2al.Services.CustomException
{
    public class AppException : Exception
    {
        public int StatusCode { get; set; } = 500;
        public AppException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
        public AppException(string message) : base(message)
        {
        }
        public AppException():base("An unexpected error occurred.")
        {

        }
    }
}
