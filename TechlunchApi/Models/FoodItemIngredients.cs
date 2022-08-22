using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TechlunchApi.Models
{
    public class FoodItemIngredients
    {
        public int Id { get; set; }

        [Required]
        public int FoodItemId { get; set; }


        [Required]
        public int IngredientId { get; set; }

        [ForeignKey("IngredientId")]
        public Ingredient IngredientFK { get; set; }


        [ForeignKey("FoodItemId")]
        public FoodItem FoodItemFK { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }


    }
}
