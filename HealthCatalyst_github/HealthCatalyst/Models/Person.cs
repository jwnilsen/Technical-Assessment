using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCatalyst.Models
{
    public partial class Person
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public int Age { get; set; }

        public string Interests { get; set; }

        public int? PictureID { get; set; }

        public virtual Picture Picture { get; set; }
    }
}
