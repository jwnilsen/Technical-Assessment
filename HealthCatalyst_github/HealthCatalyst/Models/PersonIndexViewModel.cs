using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCatalyst.Models
{
    public class PersonIndexViewModel
    {
        public IEnumerable<Person> Persons { get; set; }
        public IEnumerable<Picture> Pictures { get; set; }
    }
}