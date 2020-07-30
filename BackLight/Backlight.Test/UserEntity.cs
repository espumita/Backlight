using System;

namespace Backlight.Test {
    public class UserEntity : BacklightEntity {
        public string Name { get; set; }
        public int Age { get; set; }

        protected bool Equals(UserEntity other) {
            return Name == other.Name && Age == other.Age;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserEntity) obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Name, Age);
        }

    }
}