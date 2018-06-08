using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SimpleCommerce.Data;
using SimpleCommerce.Models;

namespace SimpleCommerce.Controllers
{
    public class ShopController : ControllerBase
    {
        public ShopController(ApplicationDbContext context) : base(context)
        {
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RemoveFromCart(int cartItemId)
        {
            string owner = User.Identity.Name;
            if (string.IsNullOrEmpty(owner))
            {
                owner = HttpContext.Session.Id;
            }
            Cart cart = GetCart(owner);

            var cartItemToRemove = cart.CartItems.Where(ci => ci.Id == cartItemId).FirstOrDefault();

            if (cartItemToRemove != null)
            {
                cart.CartItems.Remove(cartItemToRemove);
                _context.SaveChanges();
            }
            return RedirectToAction("Cart");
        }

        public IActionResult AddToCart(int productId)
        {
            string owner = User.Identity.Name;
            if (string.IsNullOrEmpty(owner))
            {
                owner = HttpContext.Session.Id;
            }
            Cart cart = GetCart(owner);
            CartItem cartItem = cart.CartItems.Where(c => c.ProductId == productId).FirstOrDefault();

            if (cartItem == null)
            {
                cartItem = new CartItem();
                cartItem.ProductId = productId;
                cartItem.Quantity = 1;
                cart.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += 1;
            }
            _context.SaveChanges();
            HttpContext.Session.SetString("CartId", cart.Id.ToString());
            return Json(cart.CartItems.Sum(ci => ci.Quantity));
        }

        public IActionResult Cart()
        {
            string owner = User.Identity.Name;
            if (string.IsNullOrEmpty(owner))
            {
                owner = HttpContext.Session.Id;
            }
            Cart cart = GetCart(owner);
            return View(cart);
        }

        [Authorize]
        public IActionResult Checkout(string errorCode = "")
        {
            ViewBag.ErrorCode = errorCode;
            string owner = User.Identity.Name;
            if (string.IsNullOrEmpty(owner))
            {
                return RedirectToAction("Login", "Account");
            }
            var order = GetOrder(owner);
            ViewBag.BillingCountries = new SelectList(_context.Regions.Where(r => r.RegionType == RegionType.Country).OrderBy(o => o.Name).ToList(), "Code", "Name");
            ViewBag.ShippingCountries = new SelectList(_context.Regions.Where(r => r.RegionType == RegionType.Country).OrderBy(o => o.Name).ToList(), "Code", "Name");
            return View(order);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(Order order)
        {

            string owner = User.Identity.Name;
            if (string.IsNullOrEmpty(owner))
            {
                return RedirectToAction("Login", "Account");
            }
            order.Cart = GetCart(owner);
            order.Owner = owner;
            order.CreateDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (order.PaymentMethod == "BankTransfer")
                {
                    order.Customer.Id = order.CustomerId.Value;
                    _context.Update(order);
                    _context.SaveChanges();
                    return RedirectToAction("CheckoutCompleted", new { orderId = order.Id });
                }
                else if (order.PaymentMethod == "CC")
                {
                    order.Customer.Id = order.CustomerId.Value;
                    _context.Orders.Update(order);
                    _context.SaveChanges();
                    return IyzicoPayment(order);
                }
            }
            ViewBag.BillingCountries = new SelectList(_context.Regions.Where(r => r.RegionType == RegionType.Country).OrderBy(o => o.Name).ToList(), "Code", "Name", order.Customer.BillingCountry);
            ViewBag.ShippingCountries = new SelectList(_context.Regions.Where(r => r.RegionType == RegionType.Country).OrderBy(o => o.Name).ToList(), "Code", "Name", order.Customer.ShippingCountry);
            return View(order);
        }
        private IActionResult IyzicoPayment(Order order)
        {
            Options options = new Options();
            options.ApiKey = "PVKewvZEgJf8UGUeYoj5FeT1nMhQk4ep";
            options.SecretKey = "Lk69QlCFmwR3mDnfnMavBtZYDB9shH93";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreateCheckoutFormInitializeRequest request = new CreateCheckoutFormInitializeRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = order.Id.ToString();
            request.Price = (order.Cart.TotalPrice * 1.18M).ToString();
            request.PaidPrice = (order.Cart.TotalPrice * 1.18M).ToString();
            request.Currency = Currency.TRY.ToString();
            request.BasketId = order.CartId.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();
            request.CallbackUrl = "https://localhost:44330/Shop/CheckoutCompleted?orderId=" + order.Id.ToString();

            List<int> enabledInstallments = new List<int>();
            enabledInstallments.Add(2);
            enabledInstallments.Add(3);
            enabledInstallments.Add(6);
            enabledInstallments.Add(9);
            request.EnabledInstallments = enabledInstallments;

            Buyer buyer = new Buyer();
            buyer.Id = order.CustomerId.ToString();
            buyer.Name = order.Customer.BillingFirstName;
            buyer.Surname = order.Customer.BillingLastName;
            buyer.GsmNumber = order.Customer.BillingPhone;
            buyer.Email = order.Customer.BillingEmail;
            buyer.IdentityNumber = order.Customer.BillingIdentityNumber;
            var email = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            buyer.LastLoginDate = user.LastLoginTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            buyer.RegistrationDate = user.RagistrationDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
            buyer.RegistrationAddress = order.Customer.BillingDistrict + " " + order.Customer.BillingStreet + " " + order.Customer.BillingAddress;

            var billingCounty = _context.Regions.FirstOrDefault(r => r.Code == order.Customer.BillingCounty && r.RegionType == RegionType.County)?.Name;
            var billingCity = _context.Regions.FirstOrDefault(r => r.Code == order.Customer.BillingCity && r.RegionType == RegionType.City)?.Name;
            var billingCountry = _context.Regions.FirstOrDefault(r => r.Code == order.Customer.BillingCountry && r.RegionType == RegionType.Country)?.Name;

            //buyer.RegistrationAddress += " " + billingCounty + "/" + billingCity + " " + billingCountry;

            buyer.Ip = HttpContext.Connection.RemoteIpAddress.ToString();
            buyer.City = billingCity;
            buyer.Country = billingCountry;
            buyer.ZipCode = order.Customer.BillingZipCode;
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = order.Customer.ShippingFirstName + " " + order.Customer.ShippingLastName;

            shippingAddress.City = _context.Regions.FirstOrDefault(r => r.Code == order.Customer.ShippingCity && r.RegionType == RegionType.City)?.Name;

            shippingAddress.Country = _context.Regions.FirstOrDefault(r => r.Code == order.Customer.ShippingCountry && r.RegionType == RegionType.Country)?.Name;

            shippingAddress.Description = order.Customer.ShippingDistrict + " " + order.Customer.ShippingStreet + " " + order.Customer.ShippingAddress;
            shippingAddress.ZipCode = order.Customer.ShippingZipCode;
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = order.Customer.FullName;
            billingAddress.City = billingCity;
            billingAddress.Country = billingCountry;
            billingAddress.Description = order.Customer.BillingDistrict + " " + order.Customer.BillingStreet + " " + order.Customer.BillingAddress;
            billingAddress.ZipCode = order.Customer.BillingZipCode;
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            foreach (var item in order.Cart.CartItems)
            {
                BasketItem basketItem = new BasketItem();
                basketItem.Id = item.Id.ToString();
                basketItem.Name = item.Product.Name + " (" + item.Quantity.ToString() + " Adet)";
                basketItem.Category1 = "Tüm Ürünler";
                basketItem.Category2 = item.Product.Category.Name;
                basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                basketItem.Price = (item.TotalPrice * 1.18M).ToString();
                basketItems.Add(basketItem);
            }

            request.BasketItems = basketItems;

            CheckoutFormInitialize checkoutFormInitialize = CheckoutFormInitialize.Create(request, options);
            if (string.IsNullOrEmpty(checkoutFormInitialize.PaymentPageUrl))
            {
                return RedirectToAction("Checkout", new { errorCode = checkoutFormInitialize.ErrorCode });
            }
            return Redirect(checkoutFormInitialize.PaymentPageUrl);
        }
        public IActionResult CheckoutCompleted(int orderId)
        {
            return View(model: orderId);
        }

        private Order GetOrder(string owner)
        {
            var cartId = GetCart(owner).Id;
            Order order = _context.Orders.Include(i => i.Cart).Include(t => t.Customer).Where(o => o.CartId == cartId).FirstOrDefault();
            if (order == null)
            {
                order = new Order();
                order.CartId = Convert.ToInt32(HttpContext.Session.GetString("CartId"));
                order.OrderStatus = OrderStatus.WaitingPaymentApproval;
                _context.Orders.Add(order);
                _context.SaveChanges();
                if (order.CustomerId == null)
                {
                    var customer = new Customer();
                    _context.Add(customer);
                    _context.SaveChanges();
                    order.CustomerId = customer.Id;
                    order.Customer = customer;
                    _context.SaveChanges();
                }
            }
            else
            {
                if (order.CustomerId == null)
                {
                    var customer = new Customer();
                    _context.Add(customer);
                    _context.SaveChanges();
                    order.CustomerId = customer.Id;
                    order.Customer = customer;
                    _context.SaveChanges();
                }
            }
            return order;
        }

        public IList<Region> GetCities(string countryCode)
        {
            var id = 0;
            if (!string.IsNullOrEmpty(countryCode))
            {
                id = _context.Regions.FirstOrDefault(r => r.Code == countryCode).Id;
            }
            var cities = _context.Regions.Where(r => r.ParentRegionId == id && r.RegionType == RegionType.City).OrderBy(o => o.Name).ToList();
            return cities;
        }

        public IList<Region> GetCounties(string cityCode)
        {
            var id = 0;
            if (!string.IsNullOrEmpty(cityCode))
            {
                id = _context.Regions.FirstOrDefault(r => r.Code == cityCode).Id;
            }
            var counties = _context.Regions.Where(r => r.ParentRegionId == id && r.RegionType == RegionType.County).OrderBy(o => o.Name).ToList();
            return counties;
        }
    }
}