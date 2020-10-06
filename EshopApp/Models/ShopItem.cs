using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EshopApp.Models
{
    public class ShopItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }


        public string TypeOfOffer { get; set; }
        public enum ETypeOfOffer { None = 0, Sale = 1, New = 2, Outlet = 3 }


        public string Image { get; set; }


        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category{ get; set; }


        [Display(Name = "SubCategory")]
        public int SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }


        [Range(1,int.MaxValue, ErrorMessage = "Price should be greater than {1}€")]
        public double Price { get; set; }


    }
}
