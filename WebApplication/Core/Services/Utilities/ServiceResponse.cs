namespace Core.Services.Utilities
{
    public class ServiceResponse
    {
        public ResponseType ResponseType { get; set; }
        public string Message { get; set; }

        public ServiceResponse(ResponseType responseType)
        {
            ResponseType = responseType;
        }

        public ServiceResponse(ResponseType responseType, string message)
        {
            ResponseType = responseType;
            Message = message;
        }
    }

    public class ServiceResponse<T>
    {
        public ResponseType ResponseType { get; set; }
        public string Message { get; set; }
        public T ResponseContent { get; set; }

        public ServiceResponse(ResponseType responseType)
        {
            ResponseType = responseType;
        }

        public ServiceResponse(ResponseType responseType, string message)
        {
            ResponseType = responseType;
            Message = message;
        }

        public ServiceResponse(ResponseType responseType, T responseContent)
        {
            ResponseType = responseType;
            ResponseContent = responseContent;
        }

        public ServiceResponse(ResponseType responseType, string message, T responseContent)
        {
            ResponseType = responseType;
            Message = message;
            ResponseContent = responseContent;
        }
    }
}