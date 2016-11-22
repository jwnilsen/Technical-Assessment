
using System.ComponentModel.DataAnnotations;

namespace HealthCatalyst.Models
{
    [MetadataType(typeof(PictureMetaData))]
    public class PictureMetaData
    {
        [StringLength(128)]
        [Display(Name = "FileName")]
        public string FileName { get; set; }
    }
}
