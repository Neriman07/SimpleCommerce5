using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            if (cartItemToRemove != null) { 
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
            } else
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
        public IActionResult Checkout()
        {
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
        
        public Order GetOrder(string owner)
        {
            Order order = _context.Orders.Where(o => o.Owner == owner).FirstOrDefault();
            if (order == null)
            {
                order = new Order();
                order.CartId = Convert.ToInt32(HttpContext.Session.GetString("CartId"));
                order.OrderStatus = OrderStatus.WaitingPaymentApproval;
                _context.Orders.Add(order);
                _context.SaveChanges();
            }
            return order;
        }
        public IList<Region> GetCities(string countryCode)
        {
            var id = _context.Regions.FirstOrDefault(r=>r.Code == countryCode).Id;
            var cities = _context.Regions.Where(r => r.ParentRegionId == id && r.RegionType == RegionType.City).OrderBy(o=>o.Name).ToList();
            return cities;
        }
    }
}