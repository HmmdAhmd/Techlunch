using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechlunchApp.ViewModels
{
    public class InventoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select ingredient")]
        public int? IngredientId { get; set; }

        [ForeignKey("IngredientId")]
        public IngredientViewModel IngredientFK { get; set; }


        [Required(ErrorMessage = "Please enter quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int? Quantity { get; set; }

        [Required(ErrorMessage = "Please enter price")]
        [Range(1, float.MaxValue, ErrorMessage = "Price must be greater than or equal to 1")]
        public float? Price { get; set; }

        [Required(ErrorMessage = "Please enter added date")]
        public DateTime? AddedOn { get; set; }
    }
}
