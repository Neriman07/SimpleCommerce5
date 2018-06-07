using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleCommerce.Models
{
    public enum OrderStatus
    {
        [Display(Name = "Ödeme Bekleniyor")]
        WaitingPaymentApproval = 1,
        [Display(Name = "Ödeme Onaylandı")]
        PaymentApproved = 2,
        [Display(Name = "Kargo için Hazırlanıyor")]
        PreparingForDelivery = 3,
        [Display(Name = "Kargoda")]
        OnShipping = 4,
        [Display(Name = "Teslimat Başarılı")]
        DeliverySuccess = 5
    }
}
