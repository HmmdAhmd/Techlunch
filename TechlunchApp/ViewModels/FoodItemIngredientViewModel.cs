using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechlunchApp.ViewModels
{
    public class FoodItemIngredientViewModel
    {
        public int Id { get; set; }

        public int FoodItemId { get; set; }

        [Required(ErrorMessage = "Please select ingredient")]
        public int IngredientId { get; set; }

        [Required(ErrorMessage = "Please enter quantity")]
        public int Quantity { get; set; }

        [ForeignKey("IngredientId")]
        public IngredientViewModel IngredientFK { get; set; }
    }
}
