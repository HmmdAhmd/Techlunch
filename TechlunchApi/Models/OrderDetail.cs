using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechlunchApi.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }


        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order OrderFK { get; set; }


        [Required]
        public int FoodItemId { get; set; }

        [ForeignKey("FoodItemId")]
        public FoodItem FoodItemFK { get; set; }



        [Required]
        public int Quantity { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public float EstimatedPrice { get; set; }


    }
}

