namespace Backlight.Api {
    public class ErrorMessages {

        public static string MethodNotAllowed { get; } = "Http method is not allowed";
        public static string EntityDeserializationError { get; } = "Enity deserialization error";
        public static string EntityIsNotConfigured { get; } = "Enity is not configured";
        public static string EntityProviderIsNotAvailable { get; } = "Enity provider is not available";
        public static string EntityIdCannotBeDeserializedFromPathError { get; } = "Error in entity id path";
    }
}