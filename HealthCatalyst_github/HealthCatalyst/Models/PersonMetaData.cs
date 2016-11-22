
using System.ComponentModel.DataAnnotations;

namespace HealthCatalyst.Models
{
    [MetadataType(typeof(PersonMetaData))]
    public partial class Person
    {
        public class PersonMetaData
        {
            [Required(ErrorMessage = "The Name cannot be blank")]
            [MaxLength(50)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required(ErrorMessage = "The Address cannot be blank")]
            [MaxLength(100)]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required(ErrorMessage = "The Age cannot be blank")]
            [Range(0, 150, ErrorMessage = "Please enter an Age between 1 and 150")]
            [Display(Name = "Age")]
            public int Age { get; set; }

            [MaxLength(100)]
            [Display(Name = "Interests")]
            public string Interests { get; set; }
        }
    }
}
