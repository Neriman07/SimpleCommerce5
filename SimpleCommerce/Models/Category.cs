using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleCommerce.Models
{
    public class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; }
        [StringLength(200)]
        [Display(Name="Resim")]
        public string Photo { get; set; }
        [Display(Name = "Ürünler")]
        public virtual ICollection<Product> Products { get; set; }

        [NotMapped]
        public ICollection<Product> PublishedProducts
        {
            get
            {
                return Products.Where(p => p.IsPublished == true).ToList();
            }
        }
    }
}
