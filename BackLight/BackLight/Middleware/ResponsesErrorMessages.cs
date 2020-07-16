namespace Backlight.Middleware {
    public class ResponsesErrorMessages {

        public static string MethodNotAllowed { get; } = "Http method is not allowed";
        public static string EntityDeserializationError { get; } = "Entity deserialization error";
        public static string EntityIsNotConfigured { get; } = "Entity is not configured";
    }
}