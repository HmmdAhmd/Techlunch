using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechlunchApp.ViewModels
{
    public class InventoryViewModel
    {
        public int Id { get; set; }

        public int IngredientId { get; set; }

        [ForeignKey("IngredientId")]
        public IngredientViewModel IngredientFK { get; set; }

        [Required]
        public int AvailableQuantity { get; set; }

        [Required]
        public float AveragePrice { get; set; }
    }
}
