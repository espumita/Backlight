using System;

namespace Backlight.Sample.Web.Api.Entities {

    public class Notes {
        public string Name { get; set; } = "Todo list 01";
        public DateTime CreationDate { get; set; } = DateTime.Today.AddDays(-10);
        public string Value { get; set; } = "This is a note example";
    }

}