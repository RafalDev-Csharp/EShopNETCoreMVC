using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EshopApp.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }

        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int ShopItemId { get; set; }

        [NotMapped]
        [ForeignKey("ShopItemId")]
        public virtual ShopItem ShopItem { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Please enter the value greater or equal {1}")]
        public int Count { get; set; }
    }
}
