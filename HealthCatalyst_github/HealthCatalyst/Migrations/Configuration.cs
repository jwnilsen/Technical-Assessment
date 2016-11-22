namespace HealthCatalyst.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using HealthCatalyst.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<HealthCatalyst.DalContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "HealthCatalyst.DalContext";
        }

        protected override void Seed(HealthCatalyst.DalContext context)
        {
            //  This method will be called after migrating to the latest version.

            context.Pictures.AddOrUpdate(
                new Picture { ID = 1, FileName = "Bobby-Ruth.jpg" },
                new Picture { ID = 2, FileName = "Cindy-Moore.jpg" },
                new Picture { ID = 3, FileName = "Dave-Jones.jpg" },
                new Picture { ID = 4, FileName = "Susan-Smith.jpg" }
            );

            context.Persons.AddOrUpdate(
                new Person { ID = 1, Name = "Bobby Ruth", Address = "335 S 2270 E, SLC, UT", Age = 7, Interests = "baseball", PictureID = 1 },
                new Person { ID = 2, Name = "Cindy Moore", Address = "2256 S 1377 E, SLC UT", Age = 34, Interests = "running, golf", PictureID = 2 },
                new Person { ID = 3, Name = "Dave Jones", Address = "6993 S 2270 E, Midvale UT", Age = 44, Interests = "golf, tennis, skiing, boating, flying", PictureID = 3 },
                new Person { ID = 4, Name = "Susan Smith", Address = "9775 S 3535 E, Sandy UT", Age = 31, Interests = "skiing, riding, running", PictureID = 4 }
            );

            context.SaveChanges();
        }
    }
}
