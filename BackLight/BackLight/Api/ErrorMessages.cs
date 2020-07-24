namespace Backlight.Api {
    public class ErrorMessages {

        public static string MethodNotAllowed { get; } = "Http method is not allowed";
        public static string EntityDeserializationError { get; } = "TypeName deserialization error";
        public static string EntityIsNotConfigured { get; } = "TypeName is not configured";
        public static string EntityProviderIsNotAvailable { get; } = "TypeName provider is not available";
    }
}