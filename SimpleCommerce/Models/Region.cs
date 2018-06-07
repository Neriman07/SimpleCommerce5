using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleCommerce.Models
{
    public class Region
    {
        public int Id { get; set; }
        [Display(Name = "Kod")]
        [StringLength(200)]
        public string Code { get; set; }
        [Display(Name = "Bölge Adı")]
        [StringLength(200)]
        public string Name { get; set; }
        [Display(Name = "Üst Bölge")]
        public int? ParentRegionId { get; set; }
        [Display(Name = "Üst Bölge")]
        [ForeignKey("ParentRegionId")]
        public Region ParentRegion { get; set; }
        [Display(Name = "Bölge Türü")]
        public RegionType RegionType { get; set; }
    }
}
