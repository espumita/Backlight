
using System;

namespace Backlight.Sample.Web.Api.Entities {
    public class Exam {
        public string Id { get; set; }
        public string Subject { get; set; } = "A Subject name";
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Grade { get; set; } = 4.99m;

    }
}