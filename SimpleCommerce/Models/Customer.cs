using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleCommerce.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Display(Name = "Fatura Adı")]
        [StringLength(200)]
        public string BillingFirstName { get; set; }
        [Display(Name = "Fatura Soyadı")]
        [StringLength(200)]
        public string BillingLastName { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return BillingFirstName + " " + BillingLastName;
            }
        }

        [Display(Name = "Fatura Kimlik No")]
        [StringLength(200)]
        public string BillingIdentityNumber { get; set; }
        [Display(Name = "Fatura Firma Adı")]
        [StringLength(200)]
        public string BillingCompanyName { get; set; }
        [Display(Name = "Fatura Ülke")]
        [StringLength(200)]
        public string BillingCountry { get; set; }
        [Display(Name = "Fatura Şehir")]
        [StringLength(200)]
        public string BillingCity { get; set; }
        [Display(Name = "Fatura İlçe")]
        [StringLength(200)]
        public string BillingCounty { get; set; }
        [Display(Name = "Fatura Mahalle/Semt")]
        [StringLength(200)]
        public string BillingDistrict { get; set; }
        [Display(Name = "Fatura Cadde/Sokak")]
        [StringLength(200)]
        public string BillingStreet { get; set; }
        [Display(Name = "Fatura Adres")]
        [StringLength(200)]
        public string BillingAddress { get; set; }
        [Display(Name = "Fatura Posta Kodu")]
        [StringLength(200)]
        public string BillingZipCode { get; set; }
        [Display(Name = "Fatura E-posta")]
        [StringLength(200)]
        public string BillingEmail { get; set; }
        [Display(Name = "Fatura Telefon")]
        [StringLength(200)]
        public string BillingPhone { get; set; }

        [Display(Name = "Teslimat Adı")]
        [StringLength(200)]
        public string ShippingFirstName { get; set; }
        [Display(Name = "Teslimat Soyadı")]
        [StringLength(200)]
        public string ShippingLastName { get; set; }
        [Display(Name = "Teslimat Kimlik No")]
        [StringLength(200)]
        public string ShippingIdentityNumber { get; set; }
        [Display(Name = "Teslimat Firma Adı")]
        [StringLength(200)]
        public string ShippingCompanyName { get; set; }
        [Display(Name = "Teslimat Ülke")]
        [StringLength(200)]
        public string ShippingCountry { get; set; }
        [Display(Name = "Teslimat Şehir")]
        [StringLength(200)]
        public string ShippingCity { get; set; }
        [Display(Name = "Teslimat İlçe")]
        [StringLength(200)]
        public string ShippingCounty { get; set; }
        [Display(Name = "Teslimat Mahalle/Semt")]
        [StringLength(200)]
        public string ShippingDistrict { get; set; }
        [Display(Name = "Teslimat Cadde/Sokak")]
        [StringLength(200)]
        public string ShippingStreet { get; set; }
        [Display(Name = "Teslimat Adres")]
        [StringLength(200)]
        public string ShippingAddress { get; set; }
        [Display(Name = "Teslimat Posta Kodu")]
        [StringLength(200)]
        public string ShippingZipCode { get; set; }
        [Display(Name = "Teslimat E-posta")]
        [StringLength(200)]
        public string ShippingEmail { get; set; }
        [Display(Name = "Teslimat Telefon")]
        [StringLength(200)]
        public string ShippingPhone { get; set; }

        [Display(Name = "Kullanıcı")]
        [StringLength(200)]
        public string UserName { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
