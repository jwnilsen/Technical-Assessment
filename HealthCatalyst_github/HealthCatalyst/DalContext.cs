
using HealthCatalyst.Models;
using System.Data.Entity;

namespace HealthCatalyst
{
    public class DalContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        public DbSet<Picture> Pictures { get; set; }
    }
}
