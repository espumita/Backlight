using System;

namespace Backlight.Sample.Web.Api.Entities {
    public class Pet {
        public string Id { get; set; }
        public string Name { get; set; } = "Entity2SubtypeName2";
        public DateTime BirthDate { get; set; } = DateTime.Today;

    }
}