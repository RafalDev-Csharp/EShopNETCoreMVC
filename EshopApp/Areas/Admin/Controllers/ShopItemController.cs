using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EshopApp.Data;
using EshopApp.Models;
using EshopApp.Models.ViewModels;
using EshopApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EshopApp.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class ShopItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public ShopItemViewModel ShopItemVM { get; set; }

        public ShopItemController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            ShopItemVM = new ShopItemViewModel()
            {
                Category = _db.Category,
                ShopItem = new Models.ShopItem()
            };

        }

        public async Task<IActionResult> Index()
        {
            var shopItems = await _db.ShopItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();           
            return View(shopItems);
        }


        //GET - CREATE
        public IActionResult Create()
        {
            return View(ShopItemVM);
        }



        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            ShopItemVM.ShopItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                return View(ShopItemVM);
            }

            _db.ShopItem.Add(ShopItemVM.ShopItem);
            await _db.SaveChangesAsync();

            //Saving image section
            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var shopItemFromDB = await _db.ShopItem.FindAsync(ShopItemVM.ShopItem.Id);

            if (files.Count > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(uploads, ShopItemVM.ShopItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                shopItemFromDB.Image = @"\images\" + ShopItemVM.ShopItem.Id + extension;
            }
            else
            {
                //no file was uploaded, use default image
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultItemImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + ShopItemVM.ShopItem.Id + ".png");
                shopItemFromDB.Image = @"\images\" + ShopItemVM.ShopItem.Id + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }








        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ShopItemVM.ShopItem = await _db.ShopItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m=>m.Id==id);
            ShopItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == ShopItemVM.ShopItem.CategoryId).ToListAsync();
            
            if (ShopItemVM.ShopItem == null)
            {
                return NotFound();
            }

            return View(ShopItemVM);
        }



        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            
            ShopItemVM.ShopItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                ShopItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == ShopItemVM.ShopItem.CategoryId).ToListAsync();
                return View(ShopItemVM);
            }

            //Saving image section
            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var shopItemFromDB = await _db.ShopItem.FindAsync(ShopItemVM.ShopItem.Id);

            if (files.Count > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete origin path file
                var imagePath = Path.Combine(webRootPath, shopItemFromDB.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }


                //Upload new file
                using (var fileStream = new FileStream(Path.Combine(uploads, ShopItemVM.ShopItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                shopItemFromDB.Image = @"\images\" + ShopItemVM.ShopItem.Id + extension_new;
            }

            shopItemFromDB.Name = ShopItemVM.ShopItem.Name;
            shopItemFromDB.Description = ShopItemVM.ShopItem.Description;
            shopItemFromDB.Price = ShopItemVM.ShopItem.Price;
            shopItemFromDB.TypeOfOffer = ShopItemVM.ShopItem.TypeOfOffer;
            shopItemFromDB.CategoryId = ShopItemVM.ShopItem.CategoryId;
            shopItemFromDB.SubCategoryId = ShopItemVM.ShopItem.SubCategoryId;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }






        //GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ShopItemVM.ShopItem = await _db.ShopItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (ShopItemVM.ShopItem == null)
            {
                return NotFound();
            }

            return View(ShopItemVM);
        }






        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ShopItemVM.ShopItem = await _db.ShopItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (ShopItemVM.ShopItem == null)
            {
                return NotFound();
            }

            return View(ShopItemVM);
        }

        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            ShopItem shopItem = await _db.ShopItem.FindAsync(id);

            if (shopItem != null)
            {
                var imagePath = Path.Combine(webRootPath, shopItem.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.ShopItem.Remove(shopItem);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }



    }
}