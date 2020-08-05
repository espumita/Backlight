namespace Backlight.Api {
    public class ErrorMessages {

        public static string MethodNotAllowed { get; } = "Http method is not allowed";
        public static string EntityIsNotConfigured { get; } = "Enity is not configured";
        public static string EntityProviderIsNotAvailable { get; } = "Enity provider is not available";
        public static string TypeCannotBeDeserializedFromPathError { get; } = "Type error in path";
        public static string EntityIdCannotBeDeserializedFromPathError { get; } = "Error in entity id path";
        public static string EntityPayloadDeserializationError { get; } = "Enity payload deserialization error";
    }
}