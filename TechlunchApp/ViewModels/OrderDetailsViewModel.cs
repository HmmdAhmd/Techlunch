using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechlunchApp.ViewModels
{
    public class OrderDetailsViewModel
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public OrderViewModel OrderFK { get; set; }


        [Required]
        public int FoodItemId { get; set; }

        [ForeignKey("FoodItemId")]
        public FoodItemViewModel FoodItemFK { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public float Price { get; set; }
    }
}
