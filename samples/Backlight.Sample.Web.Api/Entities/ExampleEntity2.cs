using System;
using System.Collections.Generic;

namespace Backlight.Sample.Web.Api.Entities {
    public class ExampleEntity2 {
        public string Name { get; set; } = "EntityName2";
        public string FirstName { get; set; } = "EntityName2";
        public string SecondName { get; set; } = "EntityName2";
        public DateTime BirthDate { get; set; } = DateTime.Today;
        public decimal AccountAmount { get; set; } = 232.322m;
        public List<ExampleEntity2SubType> Pets { get; set; } = new List<ExampleEntity2SubType> {
            new ExampleEntity2SubType { Name = "cat" },
            new ExampleEntity2SubType { Name = "dog" },
        };
    }

}