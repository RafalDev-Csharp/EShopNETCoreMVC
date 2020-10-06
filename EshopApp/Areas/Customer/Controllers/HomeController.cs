using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EshopApp.Models;
using EshopApp.Models.ViewModels;
using EshopApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using EshopApp.Utility;

namespace EshopApp.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string searchBy = null, string search = null)
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                ShopItem = await _db.ShopItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.isActive == true).ToListAsync()
            };


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var count = _db.ShoppingCart.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
            }

            foreach (var item in IndexVM.ShopItem)
            {
                if (item.Description == null)
                {
                    item.Description = "No description about this item...";
                    continue;
                }
                
                if (item.Description.Length > 200)
                {
                    item.Description = SD.ConvertToRawHtml(item.Description);
                    if (item.Description.Length > 200)
                    {
                        string str = item.Description.Substring(0, 199) + "...";
                        item.Description = str;
                    }
                }
            }

            //for (int i = 0; i < IndexVM.ShopItem.Count(); i++)
            //{
            //    if (IndexVM.ShopItem.ElementAt(i).Description == null)
            //    {
            //        IndexVM.ShopItem.ElementAt(i).Description = "No description about this item...";
            //        continue;
            //    }
            //    if (IndexVM.ShopItem.ElementAt(i).Description.Length > 200)
            //    {
            //        IndexVM.ShopItem.ElementAt(i).Description = SD.ConvertToRawHtml(IndexVM.ShopItem.ElementAt(i).Description);
            //        string str = IndexVM.ShopItem.ElementAt(i).Description.Substring(0, 199) + "...";
            //        IndexVM.ShopItem.ElementAt(i).Description = str;
            //    }
            //}


            //ShopItem.ETypeOfOffer.Sale.ToString().Contains(searchBy)


            if (search != null && searchBy == ShopItem.ETypeOfOffer.Sale.ToString())
            {
                IndexVM.ShopItem = await _db.ShopItem
                        .Where(s => (s.Name.ToLower().Contains(search.ToLower()) && s.TypeOfOffer == "1") || (s.SubCategory.Name.ToLower().Contains(search.ToLower()) && (s.TypeOfOffer == "1")))
                        .ToListAsync();
                return View(IndexVM);
            }
            else if (searchBy == "Sale" && search == null)
            {
                IndexVM.ShopItem = await _db.ShopItem
                    .Where(i => i.TypeOfOffer == "1").ToListAsync();
                return View(IndexVM);
            }
            else
            {
                if (searchBy == "None" && search != null)
                {
                    IndexVM.ShopItem = await _db.ShopItem
                        .Where(s => (s.Name.ToLower().Contains(search.ToLower()) && s.TypeOfOffer == "0") || (s.SubCategory.Name.ToLower().Contains(search.ToLower()) && (s.TypeOfOffer == "0")))
                        .ToListAsync();
                    return View(IndexVM);
                }
                else if (searchBy == "None" && search == null)
                {
                    IndexVM.ShopItem = await _db.ShopItem
                        .Where(i => i.TypeOfOffer == "0").ToListAsync();
                }
                else
                {
                    if (searchBy == "Outlet" && search != null)
                    {
                        IndexVM.ShopItem = await _db.ShopItem
                        .Where(s => (s.Name.ToLower().Contains(search.ToLower()) && s.TypeOfOffer == "3") || (s.SubCategory.Name.ToLower().Contains(search.ToLower()) && (s.TypeOfOffer == "3")))
                        .ToListAsync();
                        return View(IndexVM);
                    }
                    else if (searchBy == "Outlet" && search == null)
                    {
                        IndexVM.ShopItem = await _db.ShopItem
                            .Where(i => i.TypeOfOffer == "3").ToListAsync();
                    }
                    else
                    {
                        if (searchBy == "New" && search != null)
                        {
                            IndexVM.ShopItem = await _db.ShopItem
                        .Where(s => (s.Name.ToLower().Contains(search.ToLower()) && s.TypeOfOffer == "2") || (s.SubCategory.Name.ToLower().Contains(search.ToLower()) && (s.TypeOfOffer == "2")))
                        .ToListAsync();
                            return View(IndexVM);
                        }
                        else if (searchBy == "New" && search == null)
                        {
                            IndexVM.ShopItem = await _db.ShopItem
                                .Where(i => i.TypeOfOffer == "2").ToListAsync();
                        }
                        else
                        {
                            if (searchBy == "ALL" && search == null)
                            {
                                return View(IndexVM);
                            }
                            else if (searchBy == "ALL" && search != null)
                            {
                                IndexVM.ShopItem = await _db.ShopItem.Where(s => s.Name.ToLower().Contains(search.ToLower()) || s.SubCategory.Name.ToLower().Contains(search.ToLower())).ToListAsync();
                                return View(IndexVM);
                            }

                        }
                    }
                }
            }

            return View(IndexVM);
        }



        //Details Shopping Cart
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var shopItemFromDb = await _db.ShopItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();

            ShoppingCart cartObj = new ShoppingCart()
            {
                ShopItem = shopItemFromDb,
                ShopItemId = shopItemFromDb.Id
            };

            return View(cartObj);
        }


        //Details Add more then one item to Shopping Cart
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _db.ShoppingCart.Where(c => c.ApplicationUserId == CartObject.ApplicationUserId
                                                    && c.ShopItemId == CartObject.ShopItemId).FirstOrDefaultAsync();

                if (cartFromDb == null)
                {
                    await _db.ShoppingCart.AddAsync(CartObject);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + CartObject.Count;
                }
                await _db.SaveChangesAsync();

                var count = _db.ShoppingCart.Where(c => c.ApplicationUserId == CartObject.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var shopItemFromDb = await _db.ShopItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == CartObject.ShopItemId).FirstOrDefaultAsync();

                ShoppingCart cartObj = new ShoppingCart()
                {
                    ShopItem = shopItemFromDb,
                    ShopItemId = shopItemFromDb.Id
                };

                return View(cartObj);
            }
        }





        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
