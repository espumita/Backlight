using System;
using System.Collections.Generic;

namespace Backlight.Sample.Web.Api.Entities {
    public class User {
        public string Id { get; set; }
        public string Name { get; set; } = "George Lucas";
        public string FirstName { get; set; } = "George";
        public string SecondName { get; set; } = "Lucas";
        public DateTime BirthDate { get; set; } = new DateTime(1944, 5, 14);
        public decimal AccountAmount { get; set; } = 232.322m;
        public List<Pet> Pets { get; set; } = new List<Pet> {
            new Pet { Name = "cat" },
            new Pet { Name = "dog" },
        };
    }

}