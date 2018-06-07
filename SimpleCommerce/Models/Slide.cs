using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleCommerce.Models
{
    public class Slide
    {
        public int Id { get; set; }
        [Display(Name = "Ad")]
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [Display(Name = "Resim")]
        [StringLength(200)]
        public string Photo { get; set; }
        [StringLength(200)]
        public string Url { get; set; }
        [Display(Name = "Pozisyon")]
        public int Position { get; set; }
        public bool IsPublished { get; set; }
    }
}
