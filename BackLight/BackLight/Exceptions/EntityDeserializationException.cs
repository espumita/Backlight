using System;

namespace Backlight.Exceptions {
    public class EntityDeserializationException : Exception {
        private readonly Exception innerException;

        public EntityDeserializationException(Exception innerException) {
            this.innerException = innerException;
        }
    }
}