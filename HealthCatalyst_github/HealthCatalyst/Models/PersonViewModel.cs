
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HealthCatalyst.Models
{
    public class PersonViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "The Name cannot be blank")]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Address cannot be blank")]
        [StringLength(100)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "The Age cannot be blank")]
        [Range(0, 150, ErrorMessage = "Please enter an Age between 1 and 150")]
        [Display(Name = "Age")]
        public int? Age { get; set; }

        [StringLength(100)]
        [Display(Name = "Interests")]
        public string Interests { get; set; }

        public int? PictureID { get; set; }

        public SelectList PictureList { get; set; }

    }
}
